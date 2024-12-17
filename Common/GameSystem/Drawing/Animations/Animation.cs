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
        TimeSpan StartTime { get; }

        // Draw the card within the zone itself, if applicable
        void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation);

        // Draw additional items (such as an NPC sprite) on top of the zone
        void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale);

        bool IsDefault() => false;
        bool IsComplete();
    }

    internal class AnimationUtils 
    {
        public static void DrawZoneNPC(
            SpriteBatch spriteBatch, 
            Zone zone, 
            Vector2 position, 
            float scale, 
            Color? color = null, 
            int frame = 0, 
            Card card = null)
        {
            var npcId = card?.NPCID ?? zone.PlacedCard?.Template?.NPCID ?? 0;
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

        public static void DrawZoneCard(
            SpriteBatch spriteBatch, Zone zone, Vector2 position, float rotation, Color? color = default, Card card = null)
        {
            card ??= zone.PlacedCard?.Template;
            var texture = card?.Texture;
            if(texture == null)
            {
                return;
            }

            var bounds = texture.Value.Bounds;
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;

            spriteBatch.Draw(
                texture.Value, position + origin * Zone.CARD_DRAW_SCALE, bounds, color ?? Color.White, rotation, origin, Zone.CARD_DRAW_SCALE, SpriteEffects.None, 0);

            if(rotation == 0)
            {
                // If the card is rotated towards the player, draw its text
                CardTextRenderer.Instance.DrawCardText(spriteBatch, card, position, Zone.CARD_DRAW_SCALE, details: false);
            } 
        }

        private static Color GetNPCHealthColor(int currentHealth, int maxHealth)
        {
            float healthFraction = (float)Math.Max(0, currentHealth) / maxHealth;

            Color start = healthFraction > 0.5f ? Color.White : Color.Yellow;
            Color end = healthFraction > 0.5f ? Color.Yellow : Color.Red;

            float lerpPoint = healthFraction > 0.5f ? (2 * (healthFraction - 0.5f)) : 2 * healthFraction;

            return new Color(
                (byte)MathHelper.Lerp(end.R, start.R, lerpPoint),
                (byte)MathHelper.Lerp(end.G, start.G, lerpPoint),
                (byte)MathHelper.Lerp(end.B, start.B, lerpPoint));
        }

        public static void DrawZoneNPCStats(
            SpriteBatch spriteBatch, 
            Zone zone, 
            Vector2 position, 
            float scale, 
            float fontScale = 1f, 
            float transparency = 1f, 
            int? health = null,
            PlacedCard card = null)
        {
            // Health
            var npcId = card?.Template?.NPCID ?? zone.PlacedCard?.Template?.NPCID ?? 0;
            if(npcId == 0)
            {
                return;
            }

            health ??= zone.PlacedCard?.CurrentHealth ?? 0;
            var attack = (card ?? zone.PlacedCard).GetAttackWithModifiers(zone, null); // TODO don't explicitly pass null

            var font = FontAssets.ItemStack.Value;
            var vMargin = -4f;

            var texture = TextureCache.Instance.GetNPCTexture(npcId);
            var bounds = texture.Frame(1, Main.npcFrameCount[npcId], 0, 0);

            var localPlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>();
            var gamePlayer = localPlayer.GamePlayer;
            // right-justify health above the NPC
            {
                var zoneOffset = gamePlayer.Owns(zone) ? new Vector2(0.75f, 0.7f) : new Vector2(0.75f, 0.4f);
                var placement = ProjectedFieldUtils.Instance.WorldSpaceToScreenSpace(gamePlayer, zone, zoneOffset);
                var center = localPlayer.GameFieldPosition + placement;
                var textOffset = font.MeasureString($"{health}");
                var textPos = center - textOffset;
                var color = GetNPCHealthColor((int)health, (card?.Template ?? zone.PlacedCard.Template).MaxHealth);
                CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, $"{health}", textPos, color * transparency, fontScale);
                var heartPos = center - new Vector2(-4, textOffset.Y);
                var heartTexture = TextureCache.Instance.HeartIcon.Value;
                spriteBatch.Draw(heartTexture, heartPos, heartTexture.Bounds, Color.White * transparency, 0, default, 0.75f * fontScale, SpriteEffects.None, 0);
            }

            // left-justify attack damage above npc
            {
                var zoneOffset = gamePlayer.Owns(zone) ? new Vector2(0.1f, 0.7f) : new Vector2(0.15f, 0.4f);
                var placement = ProjectedFieldUtils.Instance.WorldSpaceToScreenSpace(gamePlayer, zone, zoneOffset);
                var center = localPlayer.GameFieldPosition + placement;
                var textOffset = font.MeasureString($"{attack.Damage}");
                var textPos = center - textOffset;
                CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, $"{attack.Damage}", textPos, Color.White * transparency, fontScale);
                var swordPos = textPos + new Vector2(textOffset.X, 0);
                var swordTexture = TextureCache.Instance.AttackIcon.Value;
                spriteBatch.Draw(swordTexture, swordPos, swordTexture.Bounds, Color.White * transparency, 0, default, fontScale, SpriteEffects.None, 0);
            }

        }
    }
}
