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
    internal class TakeDamageAnimation(Zone zone, TimeSpan startTime, TimeSpan impactTime, int startHealth, TimeSpan previousStartTime) : IAnimation
    {
        public TimeSpan StartTime { get; } = startTime;
        internal TimeSpan Duration { get; } = TimeSpan.FromSeconds(1f);

        private TimeSpan IdlePeriod { get; } = TimeSpan.FromSeconds(2f);

        private TimeSpan ElapsedTime => Main._drawInterfaceGameTime.TotalGameTime - StartTime;

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            AnimationUtils.DrawZoneCard(spriteBatch, zone, basePosition, rotation);
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            if(ElapsedTime >= impactTime) {
                // flash the npc transparent as when the player takes damage
                var sign = MathF.Sin(8f * MathF.Tau * (float)ElapsedTime.TotalSeconds);
                var health = MathHelper.Lerp(startHealth, zone.PlacedCard.CurrentHealth, 2 * (float)(ElapsedTime.TotalSeconds - impactTime.TotalSeconds));
                var transparency = sign > 0 ? 0.8f : 0.6f;
                AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition, baseScale, Color.White * transparency);
                AnimationUtils.DrawZoneNPCStats(spriteBatch, zone, basePosition, baseScale, health: (int)health);
            } else
            {
                // TODO is this too hacky - keep the same floating cycle as the previous idle animation
                var idleElapsed = Main._drawInterfaceGameTime.TotalGameTime - previousStartTime;
                var posOffset = baseScale * 3f * MathF.Sin(MathF.Tau * (float) (idleElapsed.TotalSeconds / IdlePeriod.TotalSeconds));
                AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition + new Vector2(0, posOffset), baseScale);
                AnimationUtils.DrawZoneNPCStats(spriteBatch, zone, basePosition, baseScale, health: startHealth);
            }
        }

        public bool IsComplete() =>
            Main._drawInterfaceGameTime.TotalGameTime > StartTime + Duration;
    }
}
