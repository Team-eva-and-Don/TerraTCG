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
    internal class IdleAnimation(
        Zone zone, PlacedCard placedCard = null, TimeSpan duration = default, int? healthOverride = null) : IAnimation
    {
        public TimeSpan StartTime { get; set; }

        private static TimeSpan Period { get; } = TimeSpan.FromSeconds(2f);
        private TimeSpan ElapsedTime => TCGPlayer.TotalGameTime - StartTime;

        private PlacedCard PlacedCard => placedCard ?? zone.PlacedCard;

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            if(PlacedCard == null) return;
            AnimationUtils.DrawZoneCard(
                spriteBatch, zone, basePosition, rotation, color: ZoneColor(PlacedCard), card: PlacedCard.Template);
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            if(PlacedCard == null) return;
            var posOffset = IdleHoverPos(PlacedCard, baseScale);
            var zoneColor = OverlayColor(PlacedCard);
            AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition + new Vector2(0, posOffset), baseScale, card: PlacedCard.Template, color: zoneColor);
            AnimationUtils.DrawZoneNPCStats(spriteBatch, zone, basePosition, baseScale, card: PlacedCard, health: healthOverride ?? placedCard?.CurrentHealth);
        }

        public bool IsComplete() => duration != default && ElapsedTime > duration;
        public bool IsDefault() => duration == default;

        public static float IdleHoverPos(PlacedCard placedCard, float baseScale)
        {
            var idleElapsed = TCGPlayer.TotalGameTime - placedCard.PlaceTime;
            return baseScale * 3f * MathF.Sin(MathF.Tau * (float) (idleElapsed.TotalSeconds / Period.TotalSeconds));
        }

        public static Color ZoneColor(PlacedCard placedCard) => placedCard.IsExerted ? Color.LightGray : Color.White;
        public static Color OverlayColor(PlacedCard placedCard) => placedCard.IsExerted ? Color.Gray : Color.White;
    }
}
