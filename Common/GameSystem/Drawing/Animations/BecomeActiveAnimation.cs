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
    internal class BecomeActiveAnimation(Zone zone, TimeSpan startTime) : IAnimation
    {
        public TimeSpan StartTime { get; } = startTime;

        private static TimeSpan Duration { get; } = TimeSpan.FromSeconds(0.25f);
        private TimeSpan ElapsedTime => Main._drawInterfaceGameTime.TotalGameTime - StartTime;

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            var lerpPoint = (float)(ElapsedTime.TotalSeconds / Duration.TotalSeconds);

            var zoneColor = Color.Lerp(Color.LightGray, Color.White, lerpPoint);
            AnimationUtils.DrawZoneCard(spriteBatch, zone, basePosition, rotation, zoneColor);
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            var posOffset = IdleAnimation.IdleHoverPos(zone.PlacedCard, baseScale);

            var lerpPoint = (float)(ElapsedTime.TotalSeconds / Duration.TotalSeconds);
            var zoneColor = Color.Lerp(Color.Gray, Color.White, lerpPoint);

            AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition + new Vector2(0, posOffset), baseScale, color: zoneColor);
            AnimationUtils.DrawZoneNPCStats(spriteBatch, zone, basePosition, baseScale);
        }

        public bool IsComplete() => ElapsedTime > Duration;

    }
}
