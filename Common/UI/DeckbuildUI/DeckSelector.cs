using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using TerraTCG.Common.GameSystem.Drawing;

namespace TerraTCG.Common.UI.DeckbuildUI
{
    internal class DeckSelector : UIPanel
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var textPos = new Vector2(GetInnerDimensions().X, GetInnerDimensions().Y);
            var font = FontAssets.MouseText.Value;
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, "Deck Selector:", textPos, font: font);
        }
    }
}
