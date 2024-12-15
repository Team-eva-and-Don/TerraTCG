using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.Drawing.Animations
{
    internal class ApplyModifierAnimation(Zone zone, List<ICardModifier> modifiers, TimeSpan startTime) : IAnimation
    {
        public TimeSpan StartTime => startTime;
        private TimeSpan ElapsedTime => Main._drawInterfaceGameTime.TotalGameTime - StartTime;
        private TimeSpan Period => TimeSpan.FromSeconds(1);

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            AnimationUtils.DrawZoneCard(spriteBatch, zone, basePosition, rotation);
            // TODO
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            // TODO
            var posOffset = baseScale * 3f * MathF.Sin(MathF.Tau * (float) (ElapsedTime.TotalSeconds / Period.TotalSeconds));
            AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition + new Vector2(0, posOffset), baseScale);
            AnimationUtils.DrawZoneNPCHealth(spriteBatch, zone, basePosition, baseScale);
        }

        public bool IsComplete()
        {
            // TODO
            return true;
        }
    }
}
