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

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation) =>
            AnimationUtils.DrawZoneCard(spriteBatch, zone, basePosition, rotation);

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            var posOffset = baseScale * 3f * MathF.Sin(MathF.Tau * (float) (ElapsedTime.TotalSeconds / Period.TotalSeconds));
            AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition + new Vector2(0, posOffset), baseScale);
            AnimationUtils.DrawZoneNPCHealth(spriteBatch, zone, basePosition, baseScale);
        }

        public bool IsComplete() => false;
        public bool IsDefault() => true;
    }
}
