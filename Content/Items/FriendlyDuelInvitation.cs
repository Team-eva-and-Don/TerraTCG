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
using TerraTCG.Common.Configs;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI;

namespace TerraTCG.Content.Items
{
    internal class FriendlyDuelInvitation : ModItem
    {
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 60;
            Item.useTime = 60;
			Item.width = 32;
			Item.height = 32;
            Item.rare = ItemRarityID.Orange;
        }

        public override bool? UseItem(Player player)
        {
			if (player.whoAmI == Main.myPlayer)
			{
				ModContent.GetInstance<UserInterfaces>().StartMatchmaking();
			}
            return true;
        }

		public override bool CanUseItem(Player player)
		{
			if(!ServerConfig.Instance.AllowPvPBosses && player.whoAmI == Main.myPlayer)
			{
				if(player.GetModPlayer<TCGPlayer>().Deck.Cards.Any(c => c.SubTypes.Contains(CardSubtype.BOSS)))
				{
					var msg = Language.GetTextValue("Mods.TerraTCG.Cards.Common.BossesNotAllowed");
					Main.NewText(msg, Color.Red);
					return false;
				}
			}
			return true;
		}

		public override void AddRecipes() => 
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<InvitationToDuel>(), 3)
                .AddTile(TileID.WorkBenches)
                .Register();
    }
}
