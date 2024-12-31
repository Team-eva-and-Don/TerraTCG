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
        public override string Texture => "Terraria/Images/Item_" + ItemID.Book;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 60;
            Item.useTime = 60;
        }

        public override bool? UseItem(Player player)
        {
            if(player.whoAmI == Main.myPlayer)
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
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool? UseItem(Player player)
        {
            if(player.whoAmI == Main.myPlayer)
            {
                TCGPlayer.LocalPlayer.DebugDeckbuildMode = true;
                ModContent.GetInstance<UserInterfaces>().StartDeckbuild();
            }
            return default;
        }
    }
}
