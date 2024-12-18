using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.UI.Common;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class OpponentHandElement : CustomClickUIElement
    {
        internal Vector2 CardPosition0 => Position - Vector2.UnitX *
            (CARD_WIDTH + CARD_MARGIN) * CARD_SCALE * TCGPlayer.LocalGamePlayer.Opponent.Hand.Cards.Count / 2;

        const float CARD_SCALE = 0.65f;
        const int CARD_MARGIN = 4;

        internal const int CARD_HEIGHT = 120;
        internal const int CARD_WIDTH = 90;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // No - op, draw-only element for the opponent's hand
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var gamePlayer = TCGPlayer.LocalGamePlayer?.Opponent;
            if (gamePlayer == null || gamePlayer.Hand?.Cards?.Count == 0)
            {
                return;
            }
            Vector2 currentPos = CardPosition0;

            var texture = TextureCache.Instance.CardBack;
            for (int _ = 0; _ < gamePlayer.Hand.Cards.Count; _++)
            {
                spriteBatch.Draw(texture.Value, currentPos, texture.Value.Bounds, Color.White * 0.4f, 0, default, CARD_SCALE, SpriteEffects.None, 0f);
                currentPos.X += texture.Width() * CARD_SCALE + CARD_MARGIN;
            }
            base.Draw(spriteBatch);
        }
    }
}
