using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.UI.DeckbuildUI
{
    internal class DeckbuildCardElement(Card sourceCard) : UIElement
    {
        internal Vector2 Position => new(Parent.GetInnerDimensions().X + Left.Pixels, Parent.GetInnerDimensions().Y + Top.Pixels);

        internal const float CARD_SCALE = 0.8f;
        internal const int CARD_MARGIN = 8;

        internal const int CARD_HEIGHT = 180;
        internal const int CARD_WIDTH = 135;

        private bool GetBounds(out Rectangle bounds)
        {
            bounds = sourceCard.Texture.Value.Bounds;
            var height = bounds.Height * CARD_SCALE;
            var parentInner = Parent.GetInnerDimensions();
            if(Position.Y > parentInner.Y + parentInner.Height)
            {
                return false;
            }
            if(Position.Y + height < parentInner.Y)
            {
                return false;
            }

            var bottomClip = parentInner.Y + parentInner.Height - (Position.Y + height);
            return bottomClip >= 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(!GetBounds(out var bounds))
            {
                return;
            }
            var texture = sourceCard.Texture;
            spriteBatch.Draw(texture.Value, Position, bounds, Color.White, 0, default, CARD_SCALE, SpriteEffects.None, 0f);
            CardTextRenderer.Instance.DrawCardText(spriteBatch, sourceCard, Position, CARD_SCALE);
        }
    }
}
