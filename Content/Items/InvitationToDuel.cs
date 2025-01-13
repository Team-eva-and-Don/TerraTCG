using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI;
using TerraTCG.Content.NPCs;

namespace TerraTCG.Content.Items
{
	internal class InvitationToDuel : ModItem
	{
		const int MAX_BOSS_DIST_SQ = 1020 * 1020;
		public override string Texture => "Terraria/Images/Item_" + ItemID.DiscountCard;

		public override void SetDefaults()
		{
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Blue;
		}

		private static void StartGameAndRigBossHand(IGamePlayerController myPlayer, IGamePlayerController opponent)
		{
			// Ensure the boss always starts with itself in hand after shuffling
			var bossCard = opponent.Deck.Cards[0];
			var game = ModContent.GetInstance<GameModSystem>().StartGame(myPlayer, opponent);
			if (!game.GamePlayers[1].Hand.Cards.Contains(bossCard))
			{
				// TODO is it OK to just overwrite a card like this? Probably not too many
				// practical ramifications
				game.GamePlayers[1].Hand.Cards[0] = bossCard;
			}
		}

		private static void StartDuelWithNearestBoss(Player player)
		{
			foreach(var npc in Main.npc.Where(npc=>npc.active && npc.boss && 
				Vector2.DistanceSquared(player.Center, npc.Center) < MAX_BOSS_DIST_SQ))
			{
				if(ModContent.GetInstance<NPCDeckMap>().NPCDecklists.TryGetValue(npc.netID, out var bossLists))
				{
					// Exit out of the duel dialogue if the player does not have a valid decklist
					if(!TCGPlayer.LocalPlayer.Deck.ValidateDeck())
					{
						Main.NewText(Language.GetTextValue("Mods.TerraTCG.Cards.Common.DeckNotValid"), Color.Red);
						return;
					}
					var bossList = bossLists.First();
					var myPlayer = TCGPlayer.LocalPlayer;
					var opponent = new SimpleBotPlayer()
					{
						Deck = bossList.DeckList,
						Reward = bossList.Reward,
						DeckName = bossList.Key,
					};
					StartGameAndRigBossHand(myPlayer, opponent);
				}
			}
		}

        public override bool? UseItem(Player player)
        {
            // TODO this seems to get called every frame, is that intended?
            if(player.whoAmI == Main.myPlayer && player.itemAnimation == Item.useAnimation - 1)
            {
				StartDuelWithNearestBoss(player);
            }
            return default;
        }
	}
}
