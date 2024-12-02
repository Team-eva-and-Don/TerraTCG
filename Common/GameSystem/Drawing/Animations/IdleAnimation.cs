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
        private TimeSpan Period { get; } = TimeSpan.FromSeconds(2f);
        private TimeSpan ElapsedTime => Main._drawInterfaceGameTime.TotalGameTime - startTime;

        public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
        {
            var texture = zone.PlacedCard?.Template.Texture;
            if(texture == null)
            {
                return;
            }

            var bounds = texture.Value.Bounds;
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;

            spriteBatch.Draw(
                texture.Value, basePosition + origin * Zone.CARD_DRAW_SCALE, bounds, Color.White, rotation, origin, Zone.CARD_DRAW_SCALE, SpriteEffects.None, 0);
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            var npcId = zone.PlacedCard?.Template?.NPCID ?? 0;
            var gamePlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;
            var posOffset = baseScale * 3f * MathF.Sin(MathF.Tau * (float) (ElapsedTime.TotalSeconds / Period.TotalSeconds));

            var texture = TextureCache.Instance.GetNPCTexture(npcId);
            var bounds = texture.Frame(1, Main.npcFrameCount[npcId], 0, 0);
            var origin = new Vector2(bounds.Width / 2, bounds.Height);
            var effects = gamePlayer.Owns(zone) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture.Value, basePosition + new Vector2(0, posOffset), bounds, Color.White, 0, origin, baseScale, effects, 0);
        }

        public bool IsComplete() => false;
    }
}
