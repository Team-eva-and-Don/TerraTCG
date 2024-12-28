using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TerraTCG.Common.UI.DeckbuildUI
{
    internal class DeckbuildCardList : UIPanel
    {
        public override void Update(GameTime gameTime)
        {
            if(ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
}
