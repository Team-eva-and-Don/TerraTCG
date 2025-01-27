using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.UI;

namespace TerraTCG.Content.Items
{
    internal class TerraTCGDeckbox : ModItem
    {
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 15;
			Item.width = 32;
			Item.height = 32;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool? UseItem(Player player)
        {
            // TODO this seems to get called every frame, is that intended?
            if(player.whoAmI == Main.myPlayer && player.itemAnimation == Item.useAnimation - 1)
            {
                TCGPlayer.LocalPlayer.DebugDeckbuildMode = false;
                ModContent.GetInstance<UserInterfaces>().StartDeckbuild();
            }
            return default;
        }

        public override void AddRecipes() => 
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
    }

    internal class TerraTCGDebugDeckbox : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.WaterBolt;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.noUseGraphic = true;
            Item.useTime = 15;
        }

        public override bool? UseItem(Player player)
        {
            if(player.whoAmI == Main.myPlayer && player.itemAnimation == Item.useAnimation - 1)
            {
                TCGPlayer.LocalPlayer.DebugDeckbuildMode = true;
                ModContent.GetInstance<UserInterfaces>().StartDeckbuild();
            }
            return default;
        }
    }
}
