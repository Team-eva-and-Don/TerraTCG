using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.Drawing
{
    // Certain transformations on cards are too complicated
    // to copy via the text render method, so bake the 
    // text onto the card directly where necessary.
    internal class CardWithTextRenderer : ModSystem
    {
        public static CardWithTextRenderer Instance => ModContent.GetInstance<CardWithTextRenderer>();

        // Text is rendered at 3/4 scale by default, draw at 4/3 to get full scale text
        const float CARD_SCALE = 4f / 3f;

        const int CARD_WIDTH = 180;
        const int CARD_HEIGHT = 240;
        const int CARD_MARGIN = 2;

        const int MAX_CARDS = 10;

        public RenderTarget2D CardWithTextRenderTarget { get; private set; }


        private bool shouldUpdate = false;
        private List<Card> _toRender = [];

        public List<Card> ToRender
        {
            get => _toRender;
            set
            {
                shouldUpdate = _toRender?.Count != value.Count || 
                    (_toRender?.Zip(value, (x1, x2) => x1.Name != x2.Name).Any() ?? true);
                _toRender = value;
            }
        }

        public Rectangle CardBounds(Card card)
        {
            var idx = _toRender.FindIndex(c => c.Name == card.Name);
            return new(0, idx * (CARD_HEIGHT + CARD_MARGIN), CARD_WIDTH, CARD_HEIGHT);
        }

        public void OnEnterWorld()
        {
            if(CardWithTextRenderTarget != null)
            {
                return;
            }

            CardWithTextRenderTarget = new RenderTarget2D(
                Main.graphics.GraphicsDevice,
                CARD_WIDTH,
                MAX_CARDS * (CARD_HEIGHT + CARD_MARGIN),
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);
            Main.OnPreDraw += OnPreDraw;
        }

        public override void Unload()
        {
            base.Unload();
            Main.OnPreDraw -= OnPreDraw;
        }

		private void DrawCardWithFoiling(Card card, Vector2 pos)
		{
			var textureCache = TextureCache.Instance;
			// Draw the back of the card
			Main.spriteBatch.Draw(card.Texture.Value, pos, card.Texture.Value.Bounds, Color.White, 0, default, CARD_SCALE, SpriteEffects.None, 0);

			var rotation = (float)TCGPlayer.TotalGameTime.TotalSeconds/2f + pos.Y;
			// Draw sparkles onto the card
			var sparkleBrightness = 0.05f + 0.05f * MathF.Sin(MathF.PI * rotation);
			var sparkle2Brightness = 0.05f + 0.05f * MathF.Sin(MathF.PI * rotation + MathF.PI);
			Main.spriteBatch.Draw(textureCache.Sparkles.Value, pos, card.Texture.Value.Bounds, Color.White * sparkleBrightness, 0, default, CARD_SCALE, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(textureCache.Sparkles2.Value, pos, card.Texture.Value.Bounds, Color.White * sparkle2Brightness, 0, default, CARD_SCALE, SpriteEffects.None, 0);

			// Draw foiling over the card
			var foilOrigin = 128f * new Vector2(MathF.Cos(rotation), 0.5f * Math.Abs(MathF.Sin(rotation)));
			var foilPos = new Vector2(textureCache.Foiling.Width(), textureCache.Foiling.Height()) / 2 + foilOrigin;
			var foilBounds = card.Texture.Value.Bounds;
			foilBounds.Location += new Point((int)foilPos.X, (int)foilPos.Y);
			Main.spriteBatch.Draw(textureCache.Foiling.Value, pos, foilBounds, Color.White * 0.35f, 0, default, CARD_SCALE, SpriteEffects.None, 0);

			// Draw the non-foiled parts of the card
			if (textureCache.FoilMasks.TryGetValue(card.SortType, out var mask))
			{
				Main.spriteBatch.Draw(mask.Value, pos, card.Texture.Value.Bounds, Color.White, 0, default, CARD_SCALE, SpriteEffects.None, 0);
			}

			var nameBrightness = (255 - Main.mouseTextColor) / 255f;
			var disco = Main.DiscoColor * nameBrightness;
			var nameColor = new Color(
				Main.mouseTextColor + disco.R, Main.mouseTextColor + disco.G, Main.mouseTextColor + disco.B);
			// Draw the card text
			CardTextRenderer.Instance.DrawCardText(Main.spriteBatch, card, pos, CARD_SCALE, textColor: nameColor);
		}

        private void OnPreDraw(GameTime gameTime)
        {
            //if (!shouldUpdate)
            //{
            //    return;
            //}

            Main.instance.GraphicsDevice.SetRenderTarget(CardWithTextRenderTarget);
            Main.instance.GraphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            for(int i = 0; i < Math.Min(ToRender.Count, MAX_CARDS); i++)
            {
                var card = ToRender[i];
                var pos = new Vector2(0, i * (CARD_HEIGHT + CARD_MARGIN));
				if(i == 1)
				{
					DrawCardWithFoiling(card, pos);
				} else
				{
					// Draw the back of the card
					Main.spriteBatch.Draw(card.Texture.Value, pos, card.Texture.Value.Bounds, Color.White, 0, default, CARD_SCALE, SpriteEffects.None, 0);

					// Draw the card text
					CardTextRenderer.Instance.DrawCardText(Main.spriteBatch, card, pos, CARD_SCALE);
				}
            }

            Main.spriteBatch.End();
            Main.instance.GraphicsDevice.SetRenderTarget(null);
        }
    }
}
