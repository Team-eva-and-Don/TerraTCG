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

        private static TimeSpan Period { get; } = TimeSpan.FromSeconds(2f);
        private TimeSpan ElapsedTime => Main._drawInterfaceGameTime.TotalGameTime - zone.PlacedCard.PlaceTime;

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            AnimationUtils.DrawZoneCard(spriteBatch, zone, basePosition, rotation, color: ZoneColor(zone.PlacedCard));
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            var posOffset = IdleHoverPos(zone.PlacedCard, baseScale);
            var zoneColor = OverlayColor(zone.PlacedCard);
            AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition + new Vector2(0, posOffset), baseScale, color: zoneColor);
            AnimationUtils.DrawZoneNPCStats(spriteBatch, zone, basePosition, baseScale);
        }

        public bool IsComplete() => false;
        public bool IsDefault() => true;

        public static float IdleHoverPos(PlacedCard placedCard, float baseScale)
        {
            var idleElapsed = Main._drawInterfaceGameTime.TotalGameTime - placedCard.PlaceTime;
            return baseScale * 3f * MathF.Sin(MathF.Tau * (float) (idleElapsed.TotalSeconds / Period.TotalSeconds));
        }

        public static Color ZoneColor(PlacedCard placedCard) => placedCard.IsExerted ? Color.LightGray : Color.White;
        public static Color OverlayColor(PlacedCard placedCard) => placedCard.IsExerted ? Color.Gray : Color.White;
    }
}
