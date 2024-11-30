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
    internal class GameFieldElement : UIElement
    {
        internal Vector2 Position => new(Left.Pixels, Top.Pixels);
        public override void Update(GameTime gameTime)
        {
            var gamePlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;
            if (gamePlayer == null || gamePlayer.Field?.Zones == null)
            {
                return;
            }
            var bounds = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                FieldRenderer.FIELD_WIDTH,
                FieldRenderer.FIELD_HEIGHT);
            // TODO computing this properly outside of trail and error will be a nightmare,
            // convert from screen coordinates to projected view
            var mouseX = Main.MouseScreen.X;
            var mouseY = Main.MouseScreen.Y;

            var mouseVerticalOffset = mouseY - bounds.Bottom;
            var mouseHorizontalOffset = mouseX - bounds.Center.X;

            // TODO empirically determined, need to come up with an actual formula
            var verticalGridLines = new int[] { 
                -475, -405, 
                -420, -320,
                -255, -155,
                -145, -30,
            };

            var widthScaleFactor0 = 1f;
            var widthScaleFactor1 = 65f / 90f;


            float xScale = MathHelper.Lerp(
                widthScaleFactor0, widthScaleFactor1,
                (float)(mouseVerticalOffset - verticalGridLines[7]) / (verticalGridLines[0] - verticalGridLines[7]));
            // Check the player's field
            var zones = gamePlayer.Field.Zones;
            for ( int i = 0; i < zones.Count; i++)
            {
                int yMin = i < zones.Count / 2 ? verticalGridLines[4] : verticalGridLines[6];
                int yMax = i < zones.Count / 2 ? verticalGridLines[5] : verticalGridLines[7];

                int horizontalSlot = i % (zones.Count / 2) - 1;
                float xMin = xScale * ((FieldRenderer.CARD_WIDTH + FieldRenderer.CARD_MARGIN) * horizontalSlot - FieldRenderer.CARD_WIDTH / 2);
                float xMax = xScale * ((FieldRenderer.CARD_WIDTH + FieldRenderer.CARD_MARGIN) * (horizontalSlot + 1) - FieldRenderer.CARD_WIDTH/ 2);

                Main.NewText($"{i} {horizontalSlot}: {xMin} -> {xMax}, {yMin} -> {yMax}, {mouseHorizontalOffset} {mouseVerticalOffset}");
                if (Main.mouseLeft && 
                    mouseVerticalOffset > yMin && 
                    mouseVerticalOffset < yMax &&
                    mouseHorizontalOffset > xMin &&
                    mouseHorizontalOffset < xMax)
                {
                    gamePlayer.SelectedFieldZone = gamePlayer.Field.Zones[i];
                    break;
                }

            }
            // Check the opponent's field

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var referenceTexture = TextureCache.Instance.Field;
            var texture = FieldRenderer.Instance.PerspectiveRenderTarget;
            if(texture != null)
            {
                spriteBatch.Draw(referenceTexture.Value, Position, Color.White);
                spriteBatch.Draw(texture, Position, Color.White);
            }
        }
    }
}
