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
    internal class TakeDamageAnimation(Zone zone, TimeSpan startTime, TimeSpan impactTime, IAnimation previousAnimation) : IAnimation
    {
        internal TimeSpan Duration { get; } = TimeSpan.FromSeconds(1f);

        private TimeSpan ElapsedTime => Main._drawInterfaceGameTime.TotalGameTime - startTime;

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            AnimationUtils.DrawZoneCard(spriteBatch, zone, basePosition, rotation);
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            if(ElapsedTime >= impactTime) {
                // flash the npc transparent as when the player takes damage
                var sign = MathF.Sin(8f * MathF.Tau * (float)ElapsedTime.TotalSeconds);
                var transparency = sign > 0 ? 0.8f : 0.6f;
                AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition, baseScale, Color.White * transparency);
            } else
            {
                // TODO is this too hacky?
                previousAnimation.DrawZoneOverlay(spriteBatch, basePosition, baseScale);
            }
        }

        public bool IsComplete() =>
            Main._drawInterfaceGameTime.TotalGameTime > startTime + Duration;
    }
}
