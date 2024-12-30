using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.UI;

namespace TerraTCG.Content.Items
{
    internal class TerraTCGDeckbox : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Book;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool? UseItem(Player player)
        {
            if(player.whoAmI == Main.myPlayer)
            {
                ModContent.GetInstance<UserInterfaces>().StartDeckbuild();
            }
            return default;
        }
    }
}
