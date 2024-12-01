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

        // TODO computing this properly outside of trail and error will be a nightmare,
        // convert from screen coordinates to projected view
        internal float[] rowHeights = [
            -(475f / 576f), -(405f / 576f), 
            -(420f / 575f), -(320f / 567f),
            -(255f / 576f), -(155f / 576f),
            -(145f / 565f), -(30f  / 566f),
        ];

        private bool PerspectiveQuadContainsMouse(float xMin, float yMin, float xMax, float yMax)
        {
            // TODO computing this properly outside of trail and error will be a nightmare,
            // convert from screen coordinates to projected view
            var mouseVertical = Main.MouseScreen.Y - (Position.Y + FieldRenderer.FIELD_HEIGHT);
            var mouseHorizontal = Main.MouseScreen.X - (Position.X + FieldRenderer.FIELD_WIDTH / 2);
            var widthScaleFactor0 = 1f;
            var widthScaleFactor1 = 65f / 90f;

            float xScale = MathHelper.Lerp(
                widthScaleFactor0, widthScaleFactor1,
                (float)(mouseVertical/FieldRenderer.FIELD_HEIGHT - rowHeights[7]) / (rowHeights[0] - rowHeights[7]));

            return mouseVertical > yMin && mouseVertical < yMax &&
                   mouseHorizontal > xScale * xMin && mouseHorizontal < xScale * xMax;
        }

        public override void Update(GameTime gameTime)
        {
            var gamePlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;
            if (gamePlayer == null || gamePlayer.Field?.Zones == null)
            {
                return;
            }

            // Check the player's field
            var zoneCount = gamePlayer.Field.Zones.Count;
            for ( int i = 0; i < zoneCount; i++)
            {
                float yMin = FieldRenderer.FIELD_HEIGHT * (i < zoneCount / 2 ? rowHeights[4] : rowHeights[6]);
                float yMax = FieldRenderer.FIELD_HEIGHT * (i < zoneCount / 2 ? rowHeights[5] : rowHeights[7]);

                int horizontalSlot = i % (zoneCount / 2) - 1;
                float xMin = (FieldRenderer.CARD_WIDTH + FieldRenderer.CARD_MARGIN) * horizontalSlot - FieldRenderer.CARD_WIDTH / 2;
                float xMax = (FieldRenderer.CARD_WIDTH + FieldRenderer.CARD_MARGIN) * (horizontalSlot + 1) - FieldRenderer.CARD_WIDTH / 2;

                if (PerspectiveQuadContainsMouse(xMin, yMin, xMax, yMax) && IsClicked())
                {
                    gamePlayer.SelectZone(gamePlayer.Field.Zones[i]);
                    break;
                }
            }

            // Check the opponent's field
            for ( int i = 0; i < zoneCount; i++)
            {
                float yMin = FieldRenderer.FIELD_HEIGHT * (i < zoneCount / 2 ? rowHeights[2] : rowHeights[0]);
                float yMax = FieldRenderer.FIELD_HEIGHT * (i < zoneCount / 2 ? rowHeights[3] : rowHeights[1]);

                int horizontalSlot = 1 - i % (zoneCount / 2);
                float xMin = (FieldRenderer.CARD_WIDTH + FieldRenderer.CARD_MARGIN) * horizontalSlot - FieldRenderer.CARD_WIDTH / 2;
                float xMax = (FieldRenderer.CARD_WIDTH + FieldRenderer.CARD_MARGIN) * (horizontalSlot + 1) - FieldRenderer.CARD_WIDTH/ 2;

                if (PerspectiveQuadContainsMouse(xMin, yMin, xMax, yMax) && IsClicked())
                {
                    gamePlayer.SelectZone(gamePlayer.Opponent.Field.Zones[i]);
                    break;
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
