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
    internal class MeleeAttackAnimation(Zone zone, Zone targetZone, TimeSpan startTime) : IAnimation
    {
        private TimeSpan Duration { get; } = TimeSpan.FromSeconds(1f);
        private TimeSpan ElapsedTime => Main._drawInterfaceGameTime.TotalGameTime - startTime;

        private Vector2 Destination
        {
            get
            {
                var localPlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>();
                var gamePlayer = localPlayer.GamePlayer;
                var lerpPoint = gamePlayer.Owns(targetZone) ? 0.3f : 0.8f;
                var yPlacement = ProjectedFieldUtils.Instance.GetYBoundsForZone(gamePlayer, targetZone).Lerp(lerpPoint);
                var xCenter = ProjectedFieldUtils.Instance.GetXBoundsForZone(gamePlayer, targetZone).Center;
                var scale = ProjectedFieldUtils.Instance.GetScaleFactorAt(yPlacement);
                xCenter *= scale;
                return localPlayer.GameFieldOrigin + new Vector2(xCenter, yPlacement);
            }
        }

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation) =>
            AnimationUtils.DrawZoneCard(spriteBatch, zone, basePosition, rotation);

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            var windupDuration = Duration.TotalSeconds * 0.25f;
            var swingDuration = 0.5f * Duration.TotalSeconds;
            float lerpPoint;
            if(ElapsedTime.TotalSeconds <= windupDuration)
            {
                lerpPoint = -0.25f * MathF.Sin(MathF.PI * (float) (ElapsedTime.TotalSeconds / windupDuration));
            } else if (ElapsedTime.TotalSeconds <= windupDuration + swingDuration)
            {
                lerpPoint = MathF.Sin(MathF.PI * (float) ((ElapsedTime.TotalSeconds - windupDuration) / swingDuration));
            } else
            {
                lerpPoint = -0.25f * MathF.Sin(MathF.PI * (float) ((ElapsedTime.TotalSeconds - (windupDuration + swingDuration)) / windupDuration));
            }

            // Do two walk cycles in the span of the animation
            var npcId = zone.PlacedCard?.Template?.NPCID ?? 0;
            int frame = 0;
            if(npcId > 0)
            {
                int totalFrames = 2 * Main.npcFrameCount[npcId];
                float currentFrame = MathHelper.Lerp(0, totalFrames, (float)(ElapsedTime.TotalSeconds / Duration.TotalSeconds));
                frame = (int)currentFrame % Main.npcFrameCount[npcId];
            }

            var currentX = MathHelper.Lerp(basePosition.X, Destination.X, lerpPoint);
            var currentY = MathHelper.Lerp(basePosition.Y, Destination.Y, lerpPoint);
            AnimationUtils.DrawZoneNPC(spriteBatch, zone, new(currentX, currentY), baseScale, frame: frame);
        }

        public bool IsComplete() =>
            Main._drawInterfaceGameTime.TotalGameTime > startTime + Duration;
    }
}
