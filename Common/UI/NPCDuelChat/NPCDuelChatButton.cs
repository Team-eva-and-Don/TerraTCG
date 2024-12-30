using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.UI.Common;

namespace TerraTCG.Common.UI.NPCDuelChat
{
    internal class NPCDuelChatButton : CustomClickUIElement
    {
        public float TextScale => ContainsPoint(Main.MouseScreen) ? 1f : 0.9f;

        public LocalizedText Text { get; set; }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // TODO having the button size itself could cause issues
            if(Text != null)
            {
                var font = FontAssets.MouseText.Value;
                var myDims = font.MeasureString(Text.Value);
                Width.Pixels = myDims.X;
                Height.Pixels = myDims.Y;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var innerDims = GetInnerDimensions();
            if(Text == null || Height.Pixels == 0 || Width.Pixels == 0)
            {
                return;
            }
            var font = FontAssets.MouseText;
            var chatColor = new Color(244, 222, 122) * (Main.mouseTextColor / 255f);
            var bgColor = ContainsPoint(Main.MouseScreen) ? new Color(165, 42, 42) : Color.Black;
            var pos = new Vector2(innerDims.X, innerDims.Y) + new Vector2(innerDims.Width, innerDims.Height) / 2;
            // var center = Position + new Vector2(Width.Pixels, Height.Pixels) / 2;
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, Text.Value, pos, centered: true, color: chatColor, bgColor: bgColor, font: font.Value, scale: TextScale);
        }
    }
}
