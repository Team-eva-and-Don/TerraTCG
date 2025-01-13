using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI;
using TerraTCG.Content.Items;
using TerraTCG.Content.NPCs;

namespace TerraTCG.Common.GameSystem
{
    internal interface IGamePlayerController
    {
        public GamePlayer GamePlayer {get; set;}

        public CardCollection Deck { get; set; }

        public string DeckName { get; set; }

		public NPCDuelReward Reward { get; set; }

        public void StartGame(GamePlayer player, CardGame game);

        public void EndGame();
    }

    internal class TCGPlayer : ModPlayer, IGamePlayerController
    {
        private const string SAVE_VERSION = "1"; // TagCompound format for save data

        internal static TCGPlayer LocalPlayer => Main.LocalPlayer.GetModPlayer<TCGPlayer>();
        internal static GamePlayer LocalGamePlayer => LocalPlayer.GamePlayer;
        internal static TimeSpan TotalGameTime => Main._drawInterfaceGameTime?.TotalGameTime ?? TimeSpan.FromSeconds(0);

        // TODO putting all this UI stuff here is probably not correct
        internal static float FieldTransitionPoint
        {
            get
            {
                float lerpPoint = 0;
                if(LocalGamePlayer?.Game is CardGame game)
                {
                    if(game.EndTime != default) 
                    {
                        var timeSinceGameEnd = TotalGameTime - game.EndTime;
                        var fadeOutTime = timeSinceGameEnd.TotalSeconds - 1.5f;
                        if(fadeOutTime < 0)
                        {
                            lerpPoint = 1f;
                        } else
                        {
                            lerpPoint = 1 - Math.Min(1, 2f * (float)fadeOutTime);
                        }
                    } else
                    {
                        var timeSinceGameStart = TotalGameTime - game.StartTime;
                        lerpPoint = Math.Min(1, 2f * (float)timeSinceGameStart.TotalSeconds);
                    }
                }
                return lerpPoint;
            }
        }

        public GamePlayer GamePlayer { get; set; }

        public int ActiveDeck { get; set; } = 0;
        public List<CardCollection> SavedDecks { get; set; } = [
            BotDecks.GetStarterDeck(),
            new CardCollection(),
            new CardCollection(),
            new CardCollection(),
            new CardCollection(),
            new CardCollection(),
            new CardCollection(),
            new CardCollection(),
        ];

        public Card MouseoverCard { get; set; }
        public Zone MouseoverZone { get; set; }

        public CardCollection Deck { get => SavedDecks[ActiveDeck]; set { } }

        public CardCollection Collection { get; set; } = BotDecks.GetStarterDeck();

		// Used to track deck unlock progress
		public List<string> DefeatedDecks { get; set; } = [];


		// Cards are immutable so we can't track whether an individual card is foil,
		// Instead keep a separate list of the cards that a player has that are foil
		public CardCollection FoilCollection { get; set; } = new();

        // TODO this is not the correct place to cache this info, but is the easiest
        // Place within UI coordinates that the bottom center of the player's
        // back-center game zone is drawn
        internal Vector2 GameFieldPosition { get; set; }

        // Flag for whether the player is allowed to deckbuild with cards
        // from outside their collection. Default false
        public bool DebugDeckbuildMode { get; internal set; } = false;
		public string DeckName { get; set; }
		public NPCDuelReward Reward { get; set; }

		public override void OnEnterWorld()
        {
            if(Player.whoAmI == Main.myPlayer)
            {
                ModContent.GetInstance<FieldRenderer>().OnEnterWorld();
                ModContent.GetInstance<CardWithTextRenderer>().OnEnterWorld();
            }
        }

        public void StartGame(GamePlayer player, CardGame game)
        {
            GamePlayer = player;
            ModContent.GetInstance<UserInterfaces>().StartGame();
        }

		private IEnumerable<(int, NamedNPCDeck)> UnlockedDecks =>
			ModContent.GetInstance<NPCDeckMap>().NPCDecklists
				.SelectMany(kv => kv.Value.Select(v => (kv.Key, v)))
				.Where(kv => kv.v.IsUnlocked(this));

		private NPCDuelReward HandleFirstTimeDeckWin(IGamePlayerController opponent)
		{
			// Double reward the first time you beat an opponent
			var unlockedLists = UnlockedDecks.ToList();
			DefeatedDecks.Add(opponent.DeckName);
			var newUnlockedLists = UnlockedDecks.ToList();
			// if any new lists are unlocked, Main.NewText to alert the player
			var npcsWithNewLists = newUnlockedLists
				.Where(l=>!unlockedLists.Any(l2=>l2.Item1 == l.Item1 && l2.Item2.Key == l.Item2.Key))
				.Select(c => c.Item1)
				.Select(npcId =>
				{
					// TODO is there a more elegant way than this to get names?
					var defeatedNPC = Main.npc.Where(npc => npc.active && npc.netID == npcId).FirstOrDefault();
					if(defeatedNPC is NPC npc)
					{
						return $" for {npc.FullName}";
					} else
					{
						return "";
					}
				});
			foreach (var newNPC in npcsWithNewLists)
			{
				Main.NewText($"Unlocked a new deck{newNPC}!");
			}

			return new(opponent.Reward.ItemId, opponent.Reward.Count * 2);
		}

		private void HandleTownNPCDuelEnd()
		{
            if(GamePlayer.Game.Winner == GamePlayer)
            {
				var opponentController = GamePlayer.Opponent.Controller;
				var reward = opponentController.Reward;
				if(!DefeatedDecks.Contains(opponentController.DeckName))
				{
					reward = HandleFirstTimeDeckWin(opponentController);
				}
                Player.QuickSpawnItem(
                    Player.GetSource_GiftOrReward("TerraTCG: Won Game"), 
                    reward.ItemId, 
                    reward.Count);
            }
		}

		private void HandleBossNPCDuelEnd(NPC dueledNPC)
		{
            if(GamePlayer.Game.Winner == GamePlayer)
            {
				var opponentController = GamePlayer.Opponent.Controller;
				var reward = opponentController.Reward;
				if(!DefeatedDecks.Contains(opponentController.DeckName))
				{
					reward = HandleFirstTimeDeckWin(opponentController);
				}
                Player.QuickSpawnItem(
                    Player.GetSource_GiftOrReward("TerraTCG: Won Game"), 
                    reward.ItemId, 
                    reward.Count);

				// TODO figure out why this is not recommended for multiplayer
				dueledNPC.StrikeInstantKill();
            } else
			{
				Player.KillMe(PlayerDeathReason.ByCustomReason($"{Player.name} forfeited their soul to {dueledNPC.FullName} in a card game!"), 9999, 0);
			}
		}


        public void EndGame()
        {
			// check whether the game was with a boss
			// TODO this is a bit clunky but I don't want to add more state variables
			var dueledNPCType = ModContent.GetInstance<NPCDeckMap>().NPCDecklists
				.Where(kv => kv.Value.Any(dl=>dl.Key == GamePlayer.Opponent.Controller.DeckName))
				.Select(kv=>kv.Key)
				.FirstOrDefault();

			if(dueledNPCType is int npcType)
			{
				var dueledNPC = Main.npc.Where(npc => npc.active && npc.netID == npcType).FirstOrDefault();
				if (dueledNPC.boss)
				{
					HandleBossNPCDuelEnd(dueledNPC);
				} else
				{
					HandleTownNPCDuelEnd();
				}
			}
            GamePlayer = null;
            MouseoverCard = null;
            MouseoverZone = null;
            ModContent.GetInstance<UserInterfaces>().EndGame();
        }

        public void AddCardsToCollection(List<Card> cards)
        {
            var duplicateCount = 0;
            foreach(var card in cards)
            {
                if(Collection.Cards.Where(c=>c.Name == card.Name).Count() < 2)
                {
                    Collection.Cards.Add(card);
                } else
                {
                    duplicateCount += 1;
                }
            }
        }

		internal void AddCardToFoilCollection(Card card)
		{
			if(!FoilCollection.Cards.Where(c=>c.Name == card.Name).Any())
			{
				FoilCollection.Cards.Add(card);
			} 
		}

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["version"] = SAVE_VERSION;
            try
            {
                tag.Add("collection", Collection.Serialize());
                tag.Add("foil_collection", FoilCollection.Serialize());
                tag.Add("activeDeck", ActiveDeck);
                tag.Add("defeatedDecks", DefeatedDecks);
            }
            catch (Exception e)
            {
                Mod.Logger.ErrorFormat("An error occurred while saving player collection: {0}", e.StackTrace);
            }

            for(int i = 0; i < SavedDecks.Count; i++)
            {
                try
                {
                    tag.Add($"deck_{i}", SavedDecks[i].Serialize());
                }
                catch (Exception e)
                {
                    Mod.Logger.ErrorFormat("An error occurred while saving player decks: {0}", e.StackTrace);
                }
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.ContainsKey("version") && tag.GetString("version") == SAVE_VERSION)
            {
                try
                {
                    var collection = tag.GetList<string>("collection").ToList();
                    Collection.DeSerialize(collection);
                } catch (Exception e)
                {
                    Mod.Logger.ErrorFormat("An error occurred while loading player collection: {0}", e.StackTrace);
                    Collection = BotDecks.GetStarterDeck();
                }
                if(tag.ContainsKey("foil_collection"))
				{
					try
					{
						var collection = tag.GetList<string>("foil_collection").ToList();
						FoilCollection.DeSerialize(collection);
					} catch (Exception e)
					{
						Mod.Logger.ErrorFormat("An error occurred while loading player foil collection: {0}", e.StackTrace);
						FoilCollection = new();
					}
				}
                if(tag.ContainsKey("defeatedDecks"))
                {
					// DefeatedDecks = [.. tag.GetList<string>("defeatedDecks")];
                }

                if(tag.ContainsKey("activeDeck"))
                {
                    ActiveDeck = tag.GetInt("activeDeck");
                }
                for(int i = 0; i < SavedDecks.Count; i++)
                {
                    if(!tag.ContainsKey($"deck_{i}"))
                    {
                        continue;
                    }
                    try
                    {
                        var deckList = tag.GetList<string>($"deck_{i}").ToList();
                        SavedDecks[i].DeSerialize(deckList);
                    } catch (Exception e)
                    {
                        Mod.Logger.ErrorFormat("An error occurred while loading player decklists: {0}", e.StackTrace);
                    }
                }
            }
        }

        private static Card SelectCardFromPools(params List<Card>[] pools)
        {
            foreach (var pool in pools)
            {
                if(pool.Count > 0)
                {
                    return pool[Main.rand.Next(0, pool.Count)];
                }
            }
            return null;
        }

        // Set of hacks to stop the player from accidentally dying while playing
        // unpaused in multiplayer
        public override void PreUpdateMovement()
        {
            // Set velocity to zero so you can't accidentally run off while mid-game
            if(GamePlayer != null)
            {
                Player.velocity = default;
            }
            base.PreUpdateMovement();
        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            // Prevent getting hit while in game
            return GamePlayer == null;
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            // Prevent getting hit while in game
            return GamePlayer == null;
        }

	}
}
