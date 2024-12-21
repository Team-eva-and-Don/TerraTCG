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
    internal class ActionAnimation(Zone zone) : IAnimation
    {
        public TimeSpan StartTime { get; set; }
        private TimeSpan Duration { get; } = TimeSpan.FromSeconds(1f);
        private TimeSpan ElapsedTime => TCGPlayer.TotalGameTime - StartTime;

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation) =>
            AnimationUtils.DrawZoneCard(spriteBatch, zone, basePosition, rotation, IdleAnimation.ZoneColor(zone.PlacedCard));

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            var posOffset = baseScale * (-5f + 5f * MathF.Cos(2*MathF.Tau * (float) (ElapsedTime.TotalSeconds / Duration.TotalSeconds)));
            var zoneColor = IdleAnimation.OverlayColor(zone.PlacedCard);
            AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition + new Vector2(0, posOffset), baseScale, zoneColor);
            AnimationUtils.DrawZoneNPCStats(spriteBatch, zone, basePosition, baseScale);
        }

        public bool IsComplete() => ElapsedTime > Duration;
    }
}
