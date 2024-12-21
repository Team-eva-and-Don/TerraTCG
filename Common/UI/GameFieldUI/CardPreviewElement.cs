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
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.UI.Common;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class CardPreviewElement : DraggableUIElement
    {
        internal const float CARD_SCALE = 4f / 3f;
        public override void Draw(SpriteBatch spriteBatch)
        {
            var gamePlayer = TCGPlayer.LocalGamePlayer;
            if (gamePlayer == null || gamePlayer.MouseoverCard == null)
            {
                return;
            }
            var card = gamePlayer.MouseoverCard;
            var texture = card.Texture;
            spriteBatch.Draw(texture.Value, Position, texture.Value.Bounds, Color.White * TCGPlayer.FieldTransitionPoint, 0, default, CARD_SCALE, SpriteEffects.None, 0f);
            CardTextRenderer.Instance.DrawCardText(spriteBatch, card, Position, CARD_SCALE);
        }
    }
}
