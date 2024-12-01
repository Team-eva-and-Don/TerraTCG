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
    internal class HandElement : CustomClickUIElement
    {
        internal Vector2 Position => new(Left.Pixels, Top.Pixels);

        const float CARD_SCALE = 0.5f;
        const int CARD_MARGIN = 8;

        public override void Update(GameTime gameTime)
        {
            // TODO this is a bit silly, manually implement "onclick" rather than properly setting
            // UI element bounds
            var gamePlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;
            if (gamePlayer == null || gamePlayer.Hand?.Cards?.Count == 0)
            {
                return;
            }
            for(int i = 0; i < gamePlayer.Hand.Cards.Count; i++)
            {
                var card = gamePlayer.Hand.Cards[i];
                var bounds = card.Texture.Value.Bounds;

                var scaledBounds = new Rectangle(
                    (int)(Position.X + (bounds.Width * CARD_SCALE + CARD_MARGIN) * i),
                    (int)Position.Y,
                    (int)(bounds.Width * CARD_SCALE),
                    (int)(bounds.Height * CARD_SCALE));

                if(scaledBounds.Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y)) {
                    Main.LocalPlayer.mouseInterface = true;
                    if(IsClicked())
                    {
                        gamePlayer.SelectCardInHand(card);
                        break;
                    }
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var gamePlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;
            if (gamePlayer == null || gamePlayer.Hand?.Cards?.Count == 0)
            {
                return;
            }
            Vector2 currentPos = Position;
            foreach (var card in gamePlayer.Hand.Cards)
            {
                var texture = card.Texture;
                spriteBatch.Draw(texture.Value, currentPos, texture.Value.Bounds, Color.White, 0, default, CARD_SCALE, SpriteEffects.None, 0f);
                if (card == gamePlayer.SelectedHandCard)
                {
                    // Draw a highlight over the card
                    var highlightTexture = TextureCache.Instance.ZoneHighlighted;
                    spriteBatch.Draw(highlightTexture.Value, currentPos, highlightTexture.Value.Bounds, Color.White, 0, default, 1.5f, SpriteEffects.None, 0f);
                }
                currentPos.X += card.Texture.Width() * CARD_SCALE + CARD_MARGIN;
            }
            base.Draw(spriteBatch);
        }
    }
}
