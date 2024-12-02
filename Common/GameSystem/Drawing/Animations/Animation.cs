using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.Drawing.Animations
{
    internal interface IAnimation
    {
        // Draw the card within the zone itself, if applicable
        TimeSpan StartTime { get; }
        void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation);

        // Draw additional items (such as an NPC sprite) on top of the zone
        void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale);

        bool IsComplete();
    }

    internal class AnimationUtils 
    {
        public static void DrawZoneNPC(SpriteBatch spriteBatch, Zone zone, Vector2 position, float scale, Color? color = null, int frame = 0)
        {
            var npcId = zone.PlacedCard?.Template?.NPCID ?? 0;
            if(npcId == 0)
            {
                return;
            }
            var gamePlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;

            var texture = TextureCache.Instance.GetNPCTexture(npcId);
            var bounds = texture.Frame(1, Main.npcFrameCount[npcId], 0, frame);
            var origin = new Vector2(bounds.Width / 2, bounds.Height);
            var effects = gamePlayer.Owns(zone) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture.Value, position, bounds, color ?? Color.White, 0, origin, scale, effects, 0);
        }

        public static void DrawZoneCard(SpriteBatch spriteBatch, Zone zone, Vector2 position, float rotation, Color? color = default)
        {
            var texture = zone.PlacedCard?.Template.Texture;
            if(texture == null)
            {
                return;
            }

            var bounds = texture.Value.Bounds;
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;

            spriteBatch.Draw(
                texture.Value, position + origin * Zone.CARD_DRAW_SCALE, bounds, color ?? Color.White, rotation, origin, Zone.CARD_DRAW_SCALE, SpriteEffects.None, 0);
        }

        public static void DrawZoneNPCHealth(
            SpriteBatch spriteBatch, Zone zone, Vector2 position, float scale, float fontScale = 1f, float transparency = 1f, int? health = null)
        {
            var npcId = zone.PlacedCard?.Template?.NPCID ?? 0;
            if(npcId == 0)
            {
                return;
            }

            health ??= zone.PlacedCard.CurrentHealth;

            var font = FontAssets.ItemStack.Value;
            var vMargin = -4f;

            var texture = TextureCache.Instance.GetNPCTexture(npcId);
            var bounds = texture.Frame(1, Main.npcFrameCount[npcId], 0, 0);
            var npcOffset = new Vector2(-bounds.Width / 2, bounds.Height + vMargin) * scale;

            // right-justify health above the NPC
            var textOffset = font.MeasureString($"{health}");

            var textPos = position - npcOffset - textOffset;
            // Draw a black border for the text, then the text proper
            foreach(var offset in new Vector2[] {Vector2.UnitX, Vector2.UnitY })
            {
                spriteBatch.DrawString(font, $"{health}", textPos + offset, Color.Black * transparency, 0, Vector2.Zero, fontScale, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, $"{health}", textPos - offset, Color.Black * transparency, 0, Vector2.Zero, fontScale, SpriteEffects.None, 0);
            }
            spriteBatch.DrawString(font, $"{health}", textPos, Color.White * transparency, 0, Vector2.Zero, fontScale, SpriteEffects.None, 0);

            var heartPos = position - npcOffset - new Vector2(-4, textOffset.Y);
            var heartTexture = TextureCache.Instance.HeartIcon.Value;
            spriteBatch.Draw(heartTexture, heartPos, heartTexture.Bounds, Color.White * transparency, 0, default, 0.75f * fontScale, SpriteEffects.None, 0);
        }
    }
}
