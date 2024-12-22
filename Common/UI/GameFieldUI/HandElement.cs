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
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI.Common;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class HandElement : CustomClickUIElement
    {
        internal Vector2 CardPosition0 => Position - Vector2.UnitX *
            (CARD_WIDTH + CARD_MARGIN) * TCGPlayer.LocalGamePlayer.Hand.Cards.Count / 2;

        const float CARD_SCALE = 1f;
        const int CARD_MARGIN = 8;

        internal const int CARD_HEIGHT = 180;
        internal const int CARD_WIDTH = 135;

        public override void Update(GameTime gameTime)
        {
            var gamePlayer = TCGPlayer.LocalGamePlayer;
            if (gamePlayer == null || gamePlayer.Hand?.Cards?.Count == 0)
            {
                return;
            }
            for(int i = 0; i < gamePlayer.Hand.Cards.Count; i++)
            {
                var card = gamePlayer.Hand.Cards[i];
                var bounds = card.Texture.Value.Bounds;

                var scaledBounds = new Rectangle(
                    (int)(CardPosition0.X + (bounds.Width * CARD_SCALE + CARD_MARGIN) * i),
                    (int)CardPosition0.Y,
                    (int)(bounds.Width * CARD_SCALE),
                    (int)(bounds.Height * CARD_SCALE));

                if(scaledBounds.Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y)) {
                    Main.LocalPlayer.mouseInterface = true;
                    gamePlayer.MouseoverCard = card;
                    if(IsClicked())
                    {
                        gamePlayer.SelectCardInHand(card);
                        break;
                    }
                }
            }
            base.Update(gameTime);
        }

        internal Vector2 GetCardPosition(Card card)
        {
            var cardIdx = TCGPlayer.LocalGamePlayer.Hand.Cards.IndexOf(card);
            return CardPosition0 + Vector2.UnitX * cardIdx * (CARD_WIDTH + CARD_MARGIN);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var gamePlayer = TCGPlayer.LocalGamePlayer;
            if (gamePlayer == null || gamePlayer.Hand?.Cards?.Count == 0)
            {
                return;
            }
            foreach (var card in gamePlayer.Hand.Cards)
            {
                Vector2 currentPos = GetCardPosition(card);
                var texture = card.Texture;
                spriteBatch.Draw(texture.Value, currentPos, texture.Value.Bounds, Color.White, 0, default, CARD_SCALE, SpriteEffects.None, 0f);
                if (card == gamePlayer.SelectedHandCard)
                {
                    // Draw a highlight over the card
                    var highlightTexture = TextureCache.Instance.ZoneHighlighted;
                    spriteBatch.Draw(highlightTexture.Value, currentPos, highlightTexture.Value.Bounds, Color.White, 0, default, 1.5f, SpriteEffects.None, 0f);
                }
                CardTextRenderer.Instance.DrawCardText(spriteBatch, card, currentPos, CARD_SCALE);
            }
            base.Draw(spriteBatch);
        }
    }
}
