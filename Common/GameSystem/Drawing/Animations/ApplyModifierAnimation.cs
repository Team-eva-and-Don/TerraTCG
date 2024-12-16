using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.Drawing.Animations
{
    internal class ApplyModifierAnimation(Zone zone, List<ICardModifier> modifiers, TimeSpan startTime) : IAnimation
    {
        public TimeSpan StartTime => startTime;
        private TimeSpan ElapsedTime => Main._drawInterfaceGameTime.TotalGameTime - StartTime;
        private TimeSpan Duration => TimeSpan.FromSeconds(1.25f);
        private TimeSpan Period => TimeSpan.FromSeconds(2f);

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            AnimationUtils.DrawZoneCard(spriteBatch, zone, basePosition, rotation);
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            var posOffset = baseScale * 3f * MathF.Sin(MathF.Tau * (float) (ElapsedTime.TotalSeconds / Period.TotalSeconds));
            var lerpPoint = (float)(ElapsedTime.TotalSeconds < 1f ? 0f : 4f * (ElapsedTime.TotalSeconds - 1f));
            var itemOffset = MathHelper.Lerp(48f, 0f, lerpPoint) + posOffset;
            DrawLightRays(spriteBatch, basePosition - Vector2.UnitY * itemOffset, baseScale * (1 - lerpPoint));
            AnimationUtils.DrawZoneNPC(spriteBatch, zone, basePosition + Vector2.UnitY * posOffset, baseScale);
            AnimationUtils.DrawZoneNPCStats(spriteBatch, zone, basePosition, baseScale);

            // Draw the item itself on top of everything
            var texture = modifiers[0].Texture.Value;
            var bounds = texture.Bounds;
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;
            spriteBatch.Draw(texture, basePosition - Vector2.UnitY * itemOffset, bounds,
                Color.White, 0f,
                origin, baseScale * (1 - lerpPoint), 0, 0);
        }

        // via AmuletOfManyMinions
        private void DrawLightRays(SpriteBatch spriteBatch, Vector2 position, float baseScale)
        {
            var texture = TextureCache.Instance.LightRay.Value;
            var bounds = texture.Bounds;
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;
            var baseAngle = MathF.Sin(MathF.PI * (float) (ElapsedTime.TotalSeconds / Duration.TotalSeconds));
            var rayCount = 7;
            for (int i = 0; i < rayCount; i++)
            {
                float localAngle = baseAngle + MathHelper.TwoPi * i / rayCount;
                float localIntensity = MathF.Sin(1.75f * localAngle);
                float scale = (0.5f + 0.25f * localIntensity) * baseScale;
                float brightness = 0.65f + 0.25f * localIntensity;
                Vector2 drawOffset = localAngle.ToRotationVector2() * scale * bounds.Height / 2;
                spriteBatch.Draw(texture, position + drawOffset, bounds,
                    Color.White * brightness, localAngle + MathF.PI / 2,
                    origin, scale, 0, 0);

            }
        }

        public bool IsComplete()
        {
            // TODO
            return ElapsedTime > Duration;
        }
    }
}
