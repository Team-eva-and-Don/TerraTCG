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
    internal class GameFieldElement : CustomClickUIElement
    {
        internal Vector2 Position => new(Left.Pixels, Top.Pixels);

        private bool PerspectiveQuadContainsMouse(ProjBounds xBounds, ProjBounds yBounds)
        {
            // TODO computing this properly outside of trail and error will be a nightmare,
            // convert from screen coordinates to projected view
            var mouseVertical = Main.MouseScreen.Y - (Position.Y + FieldRenderer.FIELD_HEIGHT);
            var mouseHorizontal = Main.MouseScreen.X - (Position.X + FieldRenderer.FIELD_WIDTH / 2);

            float xScale = ProjectedFieldUtils.Instance.GetScaleFactorAt(mouseVertical);
            xBounds *= xScale;

            Main.NewText($"{mouseHorizontal} {mouseVertical}, {xBounds.Min} {xBounds.Max}, {yBounds.Min}, {yBounds.Max}");

            return mouseVertical > yBounds.Min && mouseVertical < yBounds.Max &&
                   mouseHorizontal > xScale * xBounds.Min && mouseHorizontal < xScale * xBounds.Max;
        }

        public override void Update(GameTime gameTime)
        {
            var gamePlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;
            if (gamePlayer == null || gamePlayer.Field?.Zones == null)
            {
                return;
            }

            // Check both players' fields
            foreach (var zone in gamePlayer.Field.Zones.Concat(gamePlayer.Opponent.Field.Zones))
            {
                var yBounds = ProjectedFieldUtils.Instance.GetYBoundsForZone(gamePlayer, zone);
                var xBounds = ProjectedFieldUtils.Instance.GetXBoundsForZone(gamePlayer, zone);

                if (PerspectiveQuadContainsMouse(xBounds, yBounds))
                {
                    Main.LocalPlayer.mouseInterface = true;
                    if(IsClicked())
                    {
                        gamePlayer.SelectZone(zone);
                        break;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // var referenceTexture = TextureCache.Instance.Field;
            var texture = FieldRenderer.Instance.PerspectiveRenderTarget;
            if(texture != null)
            {
                // spriteBatch.Draw(referenceTexture.Value, Position, Color.White);
                spriteBatch.Draw(texture, Position, Color.White);
            }
        }
    }
}
