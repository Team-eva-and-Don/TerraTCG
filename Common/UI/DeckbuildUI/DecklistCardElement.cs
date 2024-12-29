using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.UI.DeckbuildUI
{
    internal class DecklistCardElement() : UIPanel
    {
        public Card SourceCard { get; set; }
        public int Count { get; set; }
        internal Vector2 Position => new(Parent.GetInnerDimensions().X + Left.Pixels, Parent.GetInnerDimensions().Y + Top.Pixels);

        public const int PANEL_WIDTH = 260;
        public const int PANEL_HEIGHT = 60;

        const float CARD_SCALE = 0.8f;
        private static Rectangle CARD_PICTURE_BOUNDS = new(18, 24, 102, 54);

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(Position.Y + Height.Pixels > Parent.GetInnerDimensions().Y + Parent.GetInnerDimensions().Height)
            {
                return;
            }
            if(Position.Y < Parent.GetInnerDimensions().Y)
            {
                return;
            }
            if(SourceCard is not Card sourceCard)
            {
                return;
            }

            base.Draw(spriteBatch);
            var paddingOffset = new Vector2(PaddingLeft, PaddingTop + 2);
            var texture = sourceCard.Texture;
            spriteBatch.Draw(texture.Value, Position + paddingOffset, CARD_PICTURE_BOUNDS, Color.White, 0, default, CARD_SCALE, SpriteEffects.None, 0f);
            
            var frameTexture = TextureCache.Instance.CardPreviewFrame;
            var framePos = Position + paddingOffset - new Vector2(3, 3) * CARD_SCALE;
            spriteBatch.Draw(frameTexture.Value, framePos, frameTexture.Value.Bounds, Color.White, 0, default, CARD_SCALE, SpriteEffects.None, 0f);

            var font = FontAssets.MouseText;
            var cardName = sourceCard.CardName;
            var textPos = new Vector2(CARD_PICTURE_BOUNDS.Width, CARD_PICTURE_BOUNDS.Height / 2) * CARD_SCALE + new Vector2(8f, -8f);

            var textSpace = PANEL_WIDTH - PaddingRight - PaddingLeft - textPos.X;
            var textBounds = font.Value.MeasureString(cardName);
            var textWidth = textBounds.X;
            var scale = textWidth > textSpace ? textSpace / textWidth : 1f;

            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, cardName, Position + paddingOffset + textPos, scale: scale, font: font.Value);

            var countPos = new Vector2(0, CARD_PICTURE_BOUNDS.Height - textBounds.Y);
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, $"{Count}", Position + paddingOffset + countPos, scale: scale, font: font.Value);
        }
    }
}
