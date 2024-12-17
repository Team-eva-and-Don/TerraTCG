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
    internal class IdleAnimation(Zone zone, TimeSpan startTime) : IAnimation
    {
        public TimeSpan StartTime { get; } = startTime;
        private TimeSpan Period { get; } = TimeSpan.FromSeconds(2f);
        private TimeSpan ElapsedTime => Main._drawInterfaceGameTime.TotalGameTime - StartTime;

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            var zoneColor = zone.PlacedCard.IsExerted ? Color.LightGray : Color.White;
            AnimationUtils.DrawZoneCard(spriteBatch, zone, basePosition, rotation, color: zoneColor);
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            var posOffset = baseScale * 3f * MathF.Sin(MathF.Tau * (float) (ElapsedTime.TotalSeconds / Period.TotalSeconds));
            var zoneColor = zone.PlacedCard.IsExerted ? Color.Gray : Color.White;
            AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition + new Vector2(0, posOffset), baseScale, color: zoneColor);
            AnimationUtils.DrawZoneNPCStats(spriteBatch, zone, basePosition, baseScale);
        }

        public bool IsComplete() => false;
        public bool IsDefault() => true;
    }
}
