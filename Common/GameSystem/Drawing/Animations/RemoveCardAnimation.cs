using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.Drawing.Animations
{
    internal class RemoveCardAnimation(Zone zone, Card leavingCard, TimeSpan startTime) : IAnimation
    {
        internal TimeSpan Duration { get; } = TimeSpan.FromSeconds(0.25f);

        private TimeSpan ElapsedTime => Main._drawInterfaceGameTime.TotalGameTime - startTime;

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            var transparency = Math.Max(0, 1 - (float)(ElapsedTime.TotalSeconds/ Duration.TotalSeconds));
            AnimationUtils.DrawZoneCard(spriteBatch, zone, basePosition, rotation, Color.White * transparency);
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            var scale = MathHelper.Lerp(baseScale, 0, (float) (ElapsedTime.TotalSeconds/ Duration.TotalSeconds));
            var transparency = Math.Max(0, 1 - (float)(ElapsedTime.TotalSeconds/ Duration.TotalSeconds));
            AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition, scale, Color.White * transparency);
        }

        public bool IsComplete() =>
            Main._drawInterfaceGameTime.TotalGameTime > startTime + Duration;
    }
}
