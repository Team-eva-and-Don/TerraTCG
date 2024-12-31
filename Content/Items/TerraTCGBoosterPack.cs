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
    internal class TerraTCGBoosterPack : ModItem
    {
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 50);
            Item.UseSound = SoundID.Item156;
        }

        public override bool CanUseItem(Player player)
            => player.whoAmI == Main.myPlayer &&
            !ModContent.GetInstance<UserInterfaces>().IsPackOpening();

        public override bool? UseItem(Player player)
        {
            if(player.whoAmI == Main.myPlayer && !ModContent.GetInstance<UserInterfaces>().IsPackOpening())
            {
                Item.stack -= 1;
                TCGPlayer.LocalPlayer.OpenPackAndAddToCollection();
            }
            return default;
        }
    }
}
