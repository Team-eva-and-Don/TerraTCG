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
    internal class RemoveCardAnimation(Zone zone, PlacedCard leavingCard, TimeSpan startTime) : IAnimation
    {
        public TimeSpan StartTime { get; } = startTime;
        internal TimeSpan Duration { get; } = TimeSpan.FromSeconds(0.25f);

        private TimeSpan ElapsedTime => Main._drawInterfaceGameTime.TotalGameTime - StartTime;

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            var transparency = Math.Max(0, 1 - (float)(ElapsedTime.TotalSeconds/ Duration.TotalSeconds));
            var zoneColor = IdleAnimation.ZoneColor(leavingCard);
            AnimationUtils.DrawZoneCard(spriteBatch, zone, basePosition, rotation, zoneColor * transparency);
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            var scale = MathHelper.Lerp(baseScale, 0, (float) (ElapsedTime.TotalSeconds/ Duration.TotalSeconds));
            var transparency = Math.Max(0, 1 - (float)(ElapsedTime.TotalSeconds/ Duration.TotalSeconds));
            AnimationUtils.DrawZoneNPC(
                spriteBatch, zone, basePosition, scale, Color.White * transparency, card: leavingCard.Template);
            AnimationUtils.DrawZoneNPCStats(
                spriteBatch, 
                zone, 
                basePosition, 
                baseScale, 
                transparency: transparency, 
                card: leavingCard,
                health: leavingCard.CurrentHealth);
        }

        public bool IsComplete() =>
            Main._drawInterfaceGameTime.TotalGameTime > StartTime + Duration;
    }
}
