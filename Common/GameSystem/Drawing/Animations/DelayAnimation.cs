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
    // TODO This might be a sign of poor code design, an animation that does nothing
    // for the given duration and then replaces itself with a different animation

    internal delegate IAnimation AnimationSupplier(TimeSpan startTime);
    internal class DelayAnimation(TimeSpan duration, Zone srcZone, PlacedCard placedCard, AnimationSupplier nextAnimation) : IAnimation
    {
        public TimeSpan StartTime { get; } = TCGPlayer.TotalGameTime;

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            if(placedCard != null)
            {
                AnimationUtils.DrawZoneCard(
                    spriteBatch, srcZone, basePosition, rotation, 
                    card: placedCard.Template,
                    color: IdleAnimation.ZoneColor(placedCard));
            }
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            if(placedCard != null)
            {
                var posOffset = IdleAnimation.IdleHoverPos(placedCard, baseScale);
                var zoneColor = IdleAnimation.OverlayColor(placedCard);
                AnimationUtils.DrawZoneNPC(
                    spriteBatch, srcZone, basePosition + new Vector2(0, posOffset), baseScale, 
                    card: placedCard.Template, color: zoneColor);
                AnimationUtils.DrawZoneNPCStats(
                    spriteBatch, srcZone, basePosition, baseScale, 
                    card: placedCard,
                    health: placedCard.CurrentHealth);
            }
        }

        public bool IsComplete()
        {
            if(TCGPlayer.TotalGameTime - StartTime >= duration)
            {
                srcZone.Animation = nextAnimation.Invoke(TCGPlayer.TotalGameTime);
            }
            return false;
        }
    }
}
