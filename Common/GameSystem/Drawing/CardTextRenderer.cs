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

namespace TerraTCG.Common.GameSystem.Drawing
{
    internal class CardTextRenderer : ModSystem
    {
        internal static CardTextRenderer Instance => ModContent.GetInstance<CardTextRenderer>();

        const float BaseTextScale = 0.75f;
        const float SmallTextScale = 0.65f;

        const float HPIconScale = 0.75f;
        const float MPIconScale = 0.85f;

        const int MARGIN_L = 8;
        const int MARGIN_S = 4;

        private void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color? color = null, float scale = 1f, bool centered = false)
        {
            var font = FontAssets.ItemStack.Value;
            var origin = Vector2.Zero;
            if(centered)
            {
                var size = font.MeasureString(text);
                origin = size / 2;
            }
            spriteBatch.DrawString(font, text, position, color ?? Color.White, 0, origin, scale, SpriteEffects.None, 0);
        }

        private void DrawStringWithBorder(SpriteBatch spriteBatch, string text, Vector2 position, Color? color = null, float scale = 1f, bool centered = false)
        {
            foreach(var offset in new Vector2[] { Vector2.UnitX, Vector2.UnitY })
            {
                DrawString(spriteBatch, text, position + 2 * offset, Color.Black, scale, centered);
                DrawString(spriteBatch, text, position - 2 * offset, Color.Black, scale, centered);
            }
            DrawString(spriteBatch, text, position, color, scale, centered);
        }
        public void DrawManaCost(SpriteBatch spriteBatch, int cost, Vector2 position, float scale = 1f)
        {
            var texture = TextureCache.Instance.CostIcon.Value;
            spriteBatch.Draw(texture, position, texture.Bounds, Color.White, 0, default, scale * MPIconScale, SpriteEffects.None, 0);

            // Center text on the middle of the mana star
            var textOrigin = new Vector2(texture.Bounds.Width / 2, texture.Bounds.Height * 0.67f) * MPIconScale;
            DrawStringWithBorder(spriteBatch, $"{cost}", position + textOrigin * scale, scale: scale * SmallTextScale, centered: true);
        }

        public Vector2 GetManaCostSize(int cost)
        {
            var texture = TextureCache.Instance.CostIcon.Value;
            return new Vector2(texture.Width, texture.Height) * MPIconScale;
        }
        
        private void DrawCardTopLine(SpriteBatch spriteBatch, Card card, Vector2 position, float scale = 1f)
        {
            var font = FontAssets.ItemStack.Value;
            var bounds = card.Texture.Value.Bounds;

            var nameOffset = new Vector2(MARGIN_L, MARGIN_S);
            DrawStringWithBorder(spriteBatch, card.Name, position + nameOffset, scale: scale * BaseTextScale);

            var hpIcon = TextureCache.Instance.HeartIcon.Value;
            var hpIconWidth = hpIcon.Bounds.Width * BaseTextScale;

            var hpBounds = font.MeasureString($"{card.MaxHealth}") * HPIconScale;
            var hpIconOffset = new Vector2(bounds.Width - hpIconWidth - MARGIN_L, MARGIN_L);
            var hpOffset = new Vector2(hpIconOffset.X - hpBounds.X - MARGIN_S, MARGIN_S);

            spriteBatch.Draw(hpIcon, position + hpIconOffset * scale, hpIcon.Bounds, Color.White, 0, default, scale * HPIconScale, SpriteEffects.None, 0);
            DrawStringWithBorder(spriteBatch, $"{card.MaxHealth}", position + hpOffset * scale, scale: scale * BaseTextScale);
        }

        public void DrawCardText(SpriteBatch spriteBatch, Card card, Vector2 position, float scale = 1f)
        {
            var font = FontAssets.ItemStack.Value;
            var bounds = card.Texture.Value.Bounds;
            // Top Row: Name + HP
            DrawCardTopLine(spriteBatch, card, position, scale);
            // Middle: Modifier/Ability/Attack

            // Attack
            var attack = card.Attacks[0];
            var attackRowHeight = 120;
            var manaSize = GetManaCostSize(attack.Cost);
            var manaOffset = new Vector2(2 * MARGIN_L, attackRowHeight - MARGIN_S);
            var attackTextOffset = new Vector2(2 * MARGIN_L + manaSize.X, attackRowHeight);
            DrawManaCost(spriteBatch, attack.Cost, position + manaOffset * scale, scale);
            DrawString(spriteBatch, attack.Name, position + attackTextOffset * scale, scale: scale * BaseTextScale, color: Color.Black);

            var dmgBounds = font.MeasureString($"{attack.Damage}") * BaseTextScale;
            var dmgOffset = new Vector2(bounds.Width - dmgBounds.X - 2 * MARGIN_L, attackRowHeight);
            DrawString(spriteBatch, $"{attack.Damage}", position + dmgOffset * scale, scale: scale * BaseTextScale, color: Color.Black);
        }
    }
}
