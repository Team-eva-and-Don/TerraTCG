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
    internal class PlaceCardAnimation(Zone zone, TimeSpan startTime) : IAnimation
    {
        internal TimeSpan Duration { get; } = TimeSpan.FromSeconds(0.25f);

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

            float transparency = Math.Min(1, (float)(ElapsedTime.TotalSeconds/ Duration.TotalSeconds));
            spriteBatch.Draw(
                texture.Value, basePosition + origin * Zone.CARD_DRAW_SCALE, bounds, Color.White * transparency, rotation, origin, Zone.CARD_DRAW_SCALE, SpriteEffects.None, 0);
        }

        public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
        {
            var npcId = zone.PlacedCard?.Template?.NPCID ?? 0;
            if (npcId <= 0)
            {
                return;
            }

            var gamePlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;
            var texture = TextureCache.Instance.GetNPCTexture(npcId);
            var bounds = texture.Frame(1, Main.npcFrameCount[npcId], 0, 0);
            // position given is for point-of-contact with card, attempt to line up with
            // center-bottom of frame
            var origin = new Vector2(bounds.Width / 2, bounds.Height);
            var effects = gamePlayer.Owns(zone) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            var scale = MathHelper.Lerp(0, baseScale, (float) (ElapsedTime.TotalSeconds/ Duration.TotalSeconds));
            float transparency = Math.Min(1, (float)(ElapsedTime.TotalSeconds/ Duration.TotalSeconds));
            spriteBatch.Draw(texture.Value, basePosition, bounds, Color.White * transparency, 0, origin, scale, effects, 0);
        }

        public bool IsComplete() =>
            Main._drawInterfaceGameTime.TotalGameTime > startTime + Duration;
    }
}
