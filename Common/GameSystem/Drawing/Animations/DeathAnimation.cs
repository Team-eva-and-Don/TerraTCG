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
    // TODO carrying the "start state" into each animation as a list of parameters
    // begins to get unwieldy
    internal class DeathAnimation(
        Zone zone, 
        TimeSpan impactTime, 
        int startHealth, 
        PlacedCard leavingCard) : IAnimation
    {
        public TimeSpan StartTime { get; set;  }
        internal TimeSpan Duration { get; } = TimeSpan.FromSeconds(1.25f);

        private TimeSpan FadeOutTime { get; } = impactTime + TimeSpan.FromSeconds(0.5f);

        private TimeSpan ElapsedTime => TCGPlayer.TotalGameTime - StartTime;

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            var transparency = 1f;
            if(ElapsedTime >= FadeOutTime)
            {
                var fadeProgress = (float)((ElapsedTime - FadeOutTime).TotalSeconds / (Duration - FadeOutTime).TotalSeconds);
                transparency = Math.Max(0, 1 - fadeProgress);
            }
            var zoneColor = IdleAnimation.ZoneColor(leavingCard);
            AnimationUtils.DrawZoneCard(
                spriteBatch, zone, basePosition, rotation, color: zoneColor * transparency, card:leavingCard.Template);
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            var posOffset = IdleAnimation.IdleHoverPos(leavingCard, baseScale);
            var zoneColor = IdleAnimation.OverlayColor(leavingCard);
            if(ElapsedTime >= FadeOutTime) {
                // fade the NPC out after death
                var fadeProgress = (float)((ElapsedTime - FadeOutTime).TotalSeconds / (Duration - FadeOutTime).TotalSeconds);
                var transparency = Math.Max(0, 1 - fadeProgress);
                var scale = MathHelper.Lerp(baseScale, 0, fadeProgress);
                AnimationUtils.DrawZoneNPC(
                    spriteBatch, zone, basePosition, scale, zoneColor * transparency, card: leavingCard.Template);
                AnimationUtils.DrawZoneNPCStats(
                    spriteBatch, 
                    zone, 
                    basePosition + new Vector2(0, posOffset), 
                    baseScale, 
                    health: leavingCard.CurrentHealth, 
                    card: leavingCard);
            } else if (ElapsedTime >= impactTime)
            {
                // flash the npc transparent as when the player takes damage
                var sign = MathF.Sin(8f * MathF.Tau * (float)ElapsedTime.TotalSeconds);
                var health = MathHelper.Lerp(startHealth, leavingCard.CurrentHealth, 2 * (float)(ElapsedTime.TotalSeconds - impactTime.TotalSeconds));
                var transparency = sign > 0 ? 0.8f : 0.6f;
                AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition, baseScale, zoneColor * transparency, card: leavingCard.Template);
                AnimationUtils.DrawZoneNPCStats(spriteBatch, zone, basePosition, baseScale, health: (int)health, card: leavingCard);
            } else
            {
                // TODO is this too hacky - keep the same floating cycle as the previous idle animation
                AnimationUtils.DrawZoneNPC(
                    spriteBatch, 
                    zone, 
                    basePosition + new Vector2(0, posOffset), 
                    baseScale, 
                    zoneColor,
                    card: leavingCard.Template);
                AnimationUtils.DrawZoneNPCStats(
                    spriteBatch, 
                    zone, 
                    basePosition, 
                    baseScale, 
                    health: startHealth, 
                    card: leavingCard);
            }
        }

        public bool IsComplete() =>
            TCGPlayer.TotalGameTime > StartTime + Duration;
    }
}
