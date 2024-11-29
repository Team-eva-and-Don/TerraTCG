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

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class HandElement : UIElement
    {
        internal Vector2 Position => new(Left.Pixels, Top.Pixels);

        const float CARD_SCALE = 0.5f;

        public override void Update(GameTime gameTime)
        {
            // TODO this is a bit silly, manually implement "onclick" rather than properly setting
            // UI element bounds
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
                currentPos.X += card.Texture.Width() * CARD_SCALE + 8;
            }
            base.Draw(spriteBatch);
        }
    }
}
