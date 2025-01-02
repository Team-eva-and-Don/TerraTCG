using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;

namespace TerraTCG.Common.UI.TutorialUI
{
    internal class TutorialUIPanel : UIPanel
    {
        private TimeSpan textureChangeTime;

        private Asset<Texture2D> _prevTexture;

        public Asset<Texture2D> PrevTexture { get => _prevTexture; set { _prevTexture = value; textureChangeTime = TCGPlayer.TotalGameTime; } }

        public Asset<Texture2D> Texture { get; set; }
        public string Text { get; set; }

        public override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if(Texture == null || Text == null)
            {
                return;
            }

            var innerDims = GetInnerDimensions();
            var topCenter = new Vector2(innerDims.X, innerDims.Y) + new Vector2(innerDims.Width / 2, 0);
            var scale = TutorialUIState.TUTORIAL_IMG_SCALE;
            var origin = new Vector2(Texture.Value.Width / 2, 0);

            var lerpPoint = (float)(TCGPlayer.TotalGameTime - textureChangeTime).TotalSeconds / 2f;
            var color = Color.White;
            if(lerpPoint < 1 && _prevTexture != null)
            {
                spriteBatch.Draw(_prevTexture.Value, topCenter, Texture.Value.Bounds, color, 0, origin, scale, SpriteEffects.None, 0);
                color *= lerpPoint;
            }

            spriteBatch.Draw(Texture.Value, topCenter, Texture.Value.Bounds, color, 0, origin, scale, SpriteEffects.None, 0);
            var frameTexture = TextureCache.Instance.TutorialFrame;
            origin = new Vector2(frameTexture.Value.Width / 2, 0);
            spriteBatch.Draw(frameTexture.Value, topCenter, frameTexture.Value.Bounds, Color.White, 0, origin, 1f, SpriteEffects.None, 0);

            var font = FontAssets.MouseText.Value;
            var fontPos = new Vector2(innerDims.X, innerDims.Y) + new Vector2(0, Texture.Value.Height * scale + 8);
            var textColor = Color.White * (Main.mouseTextColor / 255f);
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, Text, fontPos, textColor, font: font);

        }
    }
}
