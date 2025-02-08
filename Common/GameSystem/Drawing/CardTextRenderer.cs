using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.Drawing
{

    internal class CardTextRenderer : ModSystem
    {
        internal class BodyHeightInfo
        {
            internal float height;

            internal float modifierHeight;

            internal float skillHeight;
            internal float skillDescriptionHeight;

            internal float attackHeight;
            internal float attackDescriptionHeight;
        }

        internal static CardTextRenderer Instance => ModContent.GetInstance<CardTextRenderer>();

        const float BaseTextScale = 0.75f;
        const float SmallTextScale = 0.65f;

        const float HPIconScale = 0.75f;
        const float MPIconScale = 0.85f;

        const int MARGIN_L = 8;
        const int MARGIN_S = 4;

        float BaseTextHeight = 0;
        float SmallTextHeight = 0;

        public Vector2 MPIconSize {
             get {
                var texture = TextureCache.Instance.ManaIcon.Value;
                return new Vector2(texture.Width, texture.Height) * MPIconScale;
            } 
        }

		private List<string> _keywordModifiers;
		private List<string> KeywordModifiers
		{
			get
			{
				_keywordModifiers ??= Enum.GetNames(typeof(ModifierType))
					.Where(m => Language.Exists($"Mods.TerraTCG.Cards.ModifierNames.{m}"))
					.Select(m => Language.GetTextValue($"Mods.TerraTCG.Cards.ModifierNames.{m}"))
					.ToList();
				return _keywordModifiers;
			}
		}

        public void DrawString(
            SpriteBatch spriteBatch, string text, Vector2 position, Color? color = null, float scale = 1f, bool centered = false, DynamicSpriteFont font = null)
        {
            font ??= FontAssets.ItemStack.Value;
            var origin = Vector2.Zero;
            if(centered)
            {
                var size = font.MeasureString(text);
                origin = size / 2;
            }
            spriteBatch.DrawString(font, text, position, color ?? Color.White, 0, origin, scale, SpriteEffects.None, 0);
        }

        public void DrawStringWithBorder(
            SpriteBatch spriteBatch, string text, Vector2 position, Color? color = null, float scale = 1f, bool centered = false, DynamicSpriteFont font = null, Color? bgColor = null)
        {
			if(bgColor == null && color is Color c && c.A != 255)
			{
				bgColor = Color.Black * (c.A / 255f);
			}
            foreach(var offset in new Vector2[] { Vector2.UnitX, Vector2.UnitY })
            {
                DrawString(spriteBatch, text, position + 2 * offset * scale, bgColor ?? Color.Black, scale, centered, font);
                DrawString(spriteBatch, text, position - 2 * offset * scale, bgColor ?? Color.Black, scale, centered, font);
            }
            DrawString(spriteBatch, text, position, color, scale, centered, font);
        }

        public void DrawManaCost(SpriteBatch spriteBatch, int cost, Vector2 position, float scale = 1f)
        {
            var texture = TextureCache.Instance.ManaIcon.Value;
            spriteBatch.Draw(texture, position, texture.Bounds, Color.White, 0, default, scale * MPIconScale, SpriteEffects.None, 0);

            // Center text on the middle of the mana star
            var textOrigin = new Vector2(texture.Bounds.Width / 2, texture.Bounds.Height * 0.67f) * MPIconScale;
            var costStr = cost == -1 ? "X" : $"{cost}";
            DrawStringWithBorder(spriteBatch, costStr, position + textOrigin * scale, scale: scale * SmallTextScale, centered: true);
        }

        
        private void DrawCardTopLine(SpriteBatch spriteBatch, Card card, Vector2 position, float scale = 1f, Color nameColor = default)
        {
            var font = FontAssets.ItemStack.Value;
            var bounds = card.Texture.Value.Bounds;

            // Top Right: Max HP
            if(card.MaxHealth > 0) // non-creature cards have 0 health
            {
                var hpIcon = TextureCache.Instance.HeartIcon.Value;
                var hpIconWidth = hpIcon.Bounds.Width * BaseTextScale;

                var hpBounds = font.MeasureString($"{card.MaxHealth}") * HPIconScale;
                var hpIconOffset = new Vector2(bounds.Width - hpIconWidth - MARGIN_L, MARGIN_S);
                var hpOffset = new Vector2(hpIconOffset.X - hpBounds.X - MARGIN_S, MARGIN_S);

                spriteBatch.Draw(hpIcon, position + hpIconOffset * scale, hpIcon.Bounds, Color.White, 0, default, scale * HPIconScale, SpriteEffects.None, 0);
                DrawStringWithBorder(spriteBatch, $"{card.MaxHealth}", position + hpOffset * scale, color: nameColor, scale: scale * BaseTextScale);
            }

			// Top left: Name
			// certain names overflow the box by default, squish them a bit

            var nameOffset = new Vector2(MARGIN_L, MARGIN_S);
            DrawStringWithBorder(spriteBatch, card.CardName, position + nameOffset, color: nameColor, scale: scale * BaseTextScale);


            // Beneath Portrait: Type line
            var typelineRowHeight = 88;
            var typelineOffset = new Vector2(MARGIN_L, typelineRowHeight);
            DrawStringWithBorder(spriteBatch, card.TypeLine, position + typelineOffset * scale, color: nameColor, scale: scale * SmallTextScale);
            // Highlight if a card is expert
            if (card.SubTypes[0] == CardSubtype.EXPERT)
            {
                var expertText = card.TypeLine.Split(" ").First();
                DrawStringWithBorder(spriteBatch, expertText, position + typelineOffset * scale, color: Color.SkyBlue, scale: scale * SmallTextScale);
            }
        }

        private BodyHeightInfo GetBodyTextHeight(Card card)
        {
            // Calculate the total height of card text/abilities
            var heightInfo = new BodyHeightInfo();
            
            if(card.HasModifierText)
            {
                heightInfo.modifierHeight = heightInfo.height;
                var lineCount = card.ModifierDescription.Split('\n').Length;
                heightInfo.height += lineCount * SmallTextHeight + MARGIN_S / 2;
            }

            if(card.HasSkillText)
            {
                heightInfo.skillHeight = heightInfo.height;
                heightInfo.height += BaseTextHeight;
            }
            if(card.HasSkillDescription)
            {
                heightInfo.skillDescriptionHeight = heightInfo.height;
                var lineCount = card.SkillDescription.Split('\n').Length;
                heightInfo.height += lineCount * SmallTextHeight;
            }
            if(card.HasSkillText || card.HasSkillDescription)
            {
                heightInfo.height += MARGIN_S / 2;
            }

            if(card.HasAttackText)
            {
                heightInfo.attackHeight = heightInfo.height;
                heightInfo.height += BaseTextHeight;
            }
            if(card.HasAttackDescription)
            {
                heightInfo.attackDescriptionHeight = heightInfo.height;
                var lineCount = card.AttackDescription.Split('\n').Length;
                heightInfo.height += lineCount * SmallTextHeight;
            }

            return heightInfo;
        }

		// Draw a multiline string of text, highlighting keyworded game mechanics
		// where they appear
		private void DrawRulesText(SpriteBatch spriteBatch, float startY, string text, Vector2 position, float scale)
		{
            var font = FontAssets.ItemStack.Value;
			var rowY = startY;
			var modifierLines = text.Split("\n");
			foreach (var line in modifierLines)
			{
				var posOffset = new Vector2(1.5f * MARGIN_L, rowY);
				DrawString(spriteBatch, line, position + posOffset * scale, Color.Black, SmallTextScale * scale);
				if(line == modifierLines[0] && line.Contains(':'))
				{
					var keyword = line.Split(":")[0];
					DrawStringWithBorder(
						spriteBatch, keyword, position + posOffset * scale, Color.SkyBlue, SmallTextScale * scale);
				}
				foreach (var keyword in KeywordModifiers)
				{
					foreach (Match match in Regex.Matches(line, $"{keyword}( [0-9])?[a-z,.:]*"))
					{
						var xOffset = font.MeasureString(line[..match.Index]).X * SmallTextScale * scale;
						DrawStringWithBorder(
							spriteBatch, match.Value, position + posOffset * scale + Vector2.UnitX * xOffset, Color.SkyBlue, SmallTextScale * scale);
					}
				}
				rowY += SmallTextHeight;
			}
		}

        private void DrawBodyText(SpriteBatch spriteBatch, Card card, Vector2 position, float scale = 1f, Attack? attackOverride = null)
        {
            var font = FontAssets.ItemStack.Value;
            var bounds = card.Texture.Value.Bounds;
            var heightInfo = GetBodyTextHeight(card);
            var centerOfBody = 135;

            var startY = centerOfBody - heightInfo.height / 2;

            var baseTextX = 1.5f * MARGIN_L + MPIconSize.X;

            // Modifier
            if(card.HasModifierText)
            {
				DrawRulesText(spriteBatch, startY + heightInfo.modifierHeight, card.ModifierDescription, position, scale);
            }
            // Skill 
            if(card.HasSkillText)
            {
                var skillRowHeight = startY + heightInfo.skillHeight;
                var manaOffset = new Vector2(1.5f * MARGIN_L, skillRowHeight - MARGIN_S / 2);
                var skillTextOffset = new Vector2(baseTextX, skillRowHeight);
                var keyword = card.SkillName.Split(":")[0];
                DrawManaCost(spriteBatch, card.Skills[0].Cost, position + manaOffset * scale, scale);
                DrawString(spriteBatch, card.SkillName, position + skillTextOffset * scale, Color.Black, scale * BaseTextScale);
                DrawStringWithBorder(
                    spriteBatch, keyword, position + skillTextOffset * scale, Color.Gold, scale * BaseTextScale);
            }

            // Skill Description
            if(card.HasSkillDescription)
            {
				DrawRulesText(spriteBatch, startY + heightInfo.skillDescriptionHeight, card.SkillDescription, position, scale);
            }

            // Attack
            if(card.HasAttackText)
            {
                if (attackOverride is not Attack attack)
                {
                    attack = card.Attacks[0];    
                }
                var attackRowHeight = startY + heightInfo.attackHeight;
                var manaOffset = new Vector2(1.5f * MARGIN_L, attackRowHeight - MARGIN_S / 2);
                var attackTextOffset = new Vector2(baseTextX, attackRowHeight);
                DrawManaCost(spriteBatch, attack.Cost, position + manaOffset * scale, scale);
                DrawString(spriteBatch, card.AttackName, position + attackTextOffset * scale, Color.Black, scale * BaseTextScale);


                var attackIcon = TextureCache.Instance.AttackIcon.Value;
                var attackIconWidth = attackIcon.Bounds.Width * MPIconScale;

                var atkDmg = attack.Damage == -1 ? "X" : $"{attack.Damage}";
                var dmgBounds = font.MeasureString(atkDmg) * BaseTextScale;
                var dmgOffset = new Vector2(bounds.Width - dmgBounds.X - 1.25f * MARGIN_L - attackIconWidth, attackRowHeight);
                DrawString(spriteBatch, atkDmg, position + dmgOffset * scale, Color.Black, scale * BaseTextScale);

                var attackIconOffset = new Vector2(bounds.Width - MARGIN_L - attackIconWidth, attackRowHeight);
                spriteBatch.Draw(
                    attackIcon, position + attackIconOffset * scale, attackIcon.Bounds, Color.White, 0, default, scale * HPIconScale, SpriteEffects.None, 0);
            }
            // Attack Description
            if(card.HasAttackDescription)
            {
				DrawRulesText(spriteBatch, startY + heightInfo.attackDescriptionHeight, card.AttackDescription, position, scale);
            }
        }

        public void DrawCardText(
            SpriteBatch spriteBatch, 
            Card card, 
            Vector2 position, 
            float scale = 1f, 
            bool details = true,
			Color? textColor = null,
            Attack? attackOverride = null)
        {
            var font = FontAssets.ItemStack.Value;
            BaseTextHeight = font.MeasureString(" ").Y * BaseTextScale * 0.8f;
            SmallTextHeight = font.MeasureString(" ").Y * SmallTextScale * 0.8f;
            DrawCardTopLine(spriteBatch, card, position, scale, textColor ?? Color.White);

            if(details)
            {
                DrawBodyText(spriteBatch, card, position, scale, attackOverride);
            }
        }
    }
}
