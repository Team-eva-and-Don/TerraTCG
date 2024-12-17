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
    internal class PlaceCardAnimation(Zone zone, TimeSpan startTime) : IAnimation
    {
        public TimeSpan StartTime { get; } = startTime;
        internal TimeSpan Duration { get; } = TimeSpan.FromSeconds(0.25f);

        private TimeSpan ElapsedTime => Main._drawInterfaceGameTime.TotalGameTime - StartTime;

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            float transparency = Math.Min(1, (float)(ElapsedTime.TotalSeconds/ Duration.TotalSeconds));
            var zoneColor = zone.PlacedCard.IsExerted ? Color.LightGray : Color.White;
            AnimationUtils.DrawZoneCard(spriteBatch, zone, basePosition, rotation, zoneColor * transparency);
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            var scale = MathHelper.Lerp(0, baseScale, (float) (ElapsedTime.TotalSeconds/ Duration.TotalSeconds));
            var transparency = Math.Min(1, (float)(ElapsedTime.TotalSeconds/ Duration.TotalSeconds));
            var zoneColor = zone.PlacedCard.IsExerted ? Color.Gray : Color.White;
            AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition, scale, zoneColor * transparency);
            AnimationUtils.DrawZoneNPCStats(spriteBatch, zone, basePosition, baseScale, transparency: transparency);
        }

        public bool IsComplete() =>
            Main._drawInterfaceGameTime.TotalGameTime > StartTime + Duration;
    }
}
