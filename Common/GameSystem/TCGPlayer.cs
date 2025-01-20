using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI;
using TerraTCG.Content.Gores;
using TerraTCG.Content.Items;
using TerraTCG.Content.NPCs;

namespace TerraTCG.Common.GameSystem
{
    internal interface IGamePlayerController
    {
        public GamePlayer GamePlayer { get; set; }

        public CardCollection Deck { get; set; }

        public string DeckName { get; set; }

		public List<NPCDuelReward> Rewards { get; set; }

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

		// Cards owned by the player and available for use in deckbuilding
        public CardCollection Collection { get; set; } = BotDecks.GetStarterDeck();

		// Cards seen by the player in at least one game - can be seen in the 
		// deckbuilder but not used
        public CardCollection SeenCollection { get; set; } = BotDecks.GetStarterDeck();

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
		public List<NPCDuelReward> Rewards { get; set; }

		// Cache the info of the NPC that the player is currently fighting
		// Used in case the NPC dies between the beginning and end of the
		// fight in multiplayer
		public NPCInfoCache NPCInfo { get; set; }

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

		private List<NPCDuelReward> HandleFirstTimeDeckWin(IGamePlayerController opponent)
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
						return $"{Language.GetTextValue("Mods.TerraTCG.Cards.Common.For")} {npc.FullName}";
					} else
					{
						return "";
					}
				});
			foreach (var newNPC in npcsWithNewLists)
			{
				Main.NewText($"{Language.GetTextValue("Mods.TerraTCG.Cards.Common.UnlockedANewDeck")}{newNPC}!");
			}

			return opponent.Rewards.Select(r => new NPCDuelReward(r.ItemId, r.Count * 2)).ToList();
		}

		private void HandleTownNPCDuelEnd()
		{
            if(GamePlayer.Game.Winner == GamePlayer)
            {
				var opponentController = GamePlayer.Opponent.Controller;
				var rewards = opponentController.Rewards;
				if(!DefeatedDecks.Contains(opponentController.DeckName))
				{
					rewards = HandleFirstTimeDeckWin(opponentController);
				}
				foreach (var reward in rewards)
				{
					Player.QuickSpawnItem(
						Player.GetSource_GiftOrReward("TerraTCG: Won Game"),
						reward.ItemId,
						reward.Count);
				}
            }
		}

		private void HandleBossNPCDuelEnd(NPC dueledNPC)
		{
			// TODO this is hacky, need game unpaused for gore to spawn
			bool isPaused = Main.gamePaused;
            try
			{
				Main.gamePaused = false;
				if (GamePlayer.Game.Winner == GamePlayer)
				{
					var opponentController = GamePlayer.Opponent.Controller;
					var rewards = opponentController.Rewards;
					if (!DefeatedDecks.Contains(opponentController.DeckName))
					{
						HandleFirstTimeDeckWin(opponentController);
					}
					foreach (var reward in rewards)
					{
						Player.QuickSpawnItem(
							Player.GetSource_GiftOrReward("TerraTCG: Won Game"),
							reward.ItemId,
							reward.Count);
					}

					if (dueledNPC?.active ?? false)
					{
						SpawnCardExplosion(dueledNPC);
						dueledNPC.StrikeInstantKill();
						if(dueledNPC.netID == NPCID.EaterofWorldsHead)
						{
							KillAllEaterOfWorldsSegments(dueledNPC.whoAmI);
						}

					}
				}
				else
				{
					SpawnCardExplosion(Player);
					//TODO localize
					Player.KillMe(PlayerDeathReason.ByCustomReason($"{Player.name} forfeited their soul to {dueledNPC.FullName} in a card game!"), 9999, 0);
				}
			}
            finally
			{
				Main.gamePaused = isPaused;
			}
		}

		private static void KillAllEaterOfWorldsSegments(int headID)
		{
			// If we beat the Eater of Worlds, kill all EoW segments
			// TODO does this pose the risk of killing another instance of the EOW fight in the world?
			var eowTypes = new int[] { NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail };
			foreach(var npc in Main.npc.Where(npc=>npc.active && eowTypes.Contains(npc.netID)))
			{
				npc.StrikeInstantKill();
			}
		}

		private static void SpawnCardExplosion(Entity source)
		{
			CardCollection deck;
			if (source is Player player)
			{
				deck = player.GetModPlayer<TCGPlayer>().Deck;
			}
			else if (source is NPC npc && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
			{
				deck = ModContent.GetInstance<NPCDeckMap>().NPCDecklists[npc.netID][0].DeckList;
			}
			else
			{
				return;
			}

			for (int i = 0; i < 5; i++)
			{
				var card = Main.rand.NextFromList([..deck.Cards]);
				int cardGore = card.GoreType;
				var launchVelocity = new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-6, -3));
				Gore.NewGore(source.GetSource_Death(), source.position, source.velocity + launchVelocity, cardGore);
			}
		}

        public void EndGame()
        {
			// check whether the game was with a boss
			// TODO this is a bit clunky but I don't want to add more state variables

			if(NPCInfo.IsBoss)
			{
				var dueledNPC = Main.npc.Where(npc => npc.active && npc.netID == NPCInfo.NpcId).FirstOrDefault();
				HandleBossNPCDuelEnd(dueledNPC);

			} else
			{
				HandleTownNPCDuelEnd();
			}
			var allSeenCards = GamePlayer.Game.Turns
				.SelectMany(t => t.ActionLog)
				.Select(l => l.Card)
				.Where(c => c != null);
			AddCardsToSeenCollection(allSeenCards);
            GamePlayer = null;
            MouseoverCard = null;
            MouseoverZone = null;
			NPCInfo = default;
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

        public void AddCardsToSeenCollection(IEnumerable<Card> cards)
        {
            foreach(var card in cards)
            {
                if(!SeenCollection.Cards.Contains(card))
                {
                    SeenCollection.Cards.Add(card);
                }             
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
                tag.Add("seen_collection", SeenCollection.Serialize());
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

		private void TryLoadCollection(TagCompound tag, string key, CardCollection destination, CardCollection backup = null)
		{
			if(tag.ContainsKey(key))
			{
				try
				{
					var collection = tag.GetList<string>(key).ToList();
					destination.DeSerialize(collection);
				} catch (Exception e)
				{
					Mod.Logger.ErrorFormat("An error occurred while loading a player collection: {0}", e.StackTrace);
					destination.Cards = backup?.Cards ?? [];
				}
			}
		}

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.ContainsKey("version") && tag.GetString("version") == SAVE_VERSION)
            {
				TryLoadCollection(tag, "collection", Collection, BotDecks.GetStarterDeck());
				TryLoadCollection(tag, "foil_collection", FoilCollection);
				TryLoadCollection(tag, "seen_collection", SeenCollection);
                for(int i = 0; i < SavedDecks.Count; i++)
                {
					TryLoadCollection(tag, $"deck_{i}", SavedDecks[i]);
                }

                if(tag.ContainsKey("defeatedDecks"))
                {
					DefeatedDecks = [.. tag.GetList<string>("defeatedDecks")];
                }

                if(tag.ContainsKey("activeDeck"))
                {
                    ActiveDeck = tag.GetInt("activeDeck");
                }
            }
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

		public override void SetControls()
		{
			// Unset any directional controls while game is in progress to
			// prevent rubberbanding in multiplayer
			if(GamePlayer != null)
			{
				Player.controlUp = false;
				Player.controlLeft = false;
				Player.controlDown = false;
				Player.controlRight = false;
				Player.controlJump = false;
				Player.controlUseItem = false;
				Player.controlUseTile = false;
				Player.controlThrow = false;
				Player.controlInv = false;
				Player.controlHook = false;
				Player.controlTorch = false;
				Player.controlSmart = false;
				Player.controlMount = false;
				Player.controlQuickHeal = false;
				Player.controlQuickMana = false;
				Player.controlCreativeMenu = false;
				Player.mapStyle = false;
				Player.mapAlphaDown = false;
				Player.mapAlphaUp = false;
				Player.mapFullScreen = false;
				Player.mapZoomIn = false;
				Player.mapZoomOut = false;
			}
		}

	}
}
