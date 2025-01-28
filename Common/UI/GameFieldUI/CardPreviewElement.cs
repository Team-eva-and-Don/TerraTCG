using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using TerraTCG.Common.Configs;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.UI.Common;
using TerraTCG.Common.UI.DeckbuildUI;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class CardPreviewElement : DraggableUIElement
    {
        internal const float CARD_SCALE = 4f / 3f;
        private string BuffIconTooltip;

		public static string GetCardDetailsToolTip(Zone zone)
		{
            var modifiers = zone.GetKeywordModifiers() ?? [];
            var tooltipTexts = modifiers
                .OrderBy(kv => kv.Key)
                .Select(kv => Language.GetText($"Mods.TerraTCG.Cards.Modifiers.{kv.Key}").Format($"{kv.Value}"));

			var equipments = zone.PlacedCard?.CardModifiers.Where(m => m.Source == CardSubtype.EQUIPMENT)
				.GroupBy(m => m.SourceCard)
				.Select(group => $"  {group.Count()}x {group.First().SourceCard.CardName}") ?? [];

			if(equipments.Any())
			{
				equipments = new string[] { $"{Language.GetTextValue("Mods.TerraTCG.Cards.Types.EQUIPMENT")}:" }.Concat(equipments);
			}
            return string.Join("\n", tooltipTexts.Concat(equipments));
		}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var localPlayer = TCGPlayer.LocalPlayer;
			BuffIconTooltip = localPlayer.MouseoverZone is Zone zone ? GetCardDetailsToolTip(zone) : "";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var localPlayer = TCGPlayer.LocalPlayer;
            if (localPlayer == null || localPlayer.MouseoverCard == null || !ModContent.GetInstance<ClientConfig>().ShowCardPreview)
            {
                return;
            }
            var fadePoint = localPlayer.GamePlayer == null ? 1f : TCGPlayer.FieldTransitionPoint;
            var card = localPlayer.MouseoverCard;
            var texture = card.Texture;
            var attackOverride = localPlayer.MouseoverZone?.PlacedCard?.GetAttackWithModifiers(localPlayer.MouseoverZone, null);
			FoilCardRenderer.DrawCard(spriteBatch, card, Position, Color.White * fadePoint, CARD_SCALE, 0);

            if(attackOverride != null)
            {
                var center = Position + new Vector2(2, texture.Value.Height * CARD_SCALE);
                foreach(var modifier in localPlayer.MouseoverZone.GetKeywordModifiers().Keys.Order())
                {
                    var iconTexture = TextureCache.Instance.ModifierIconTextures[modifier].Value;
                    var origin = new Vector2(0, iconTexture.Height);
                    spriteBatch.Draw(iconTexture, center, iconTexture.Bounds, Color.White, 0, origin, 1, SpriteEffects.None, 0);
                    center.X += iconTexture.Width + 2;
                }
            }

            // This elem is used in both the fancy and normal UI, regular "ContainsPoint"
            // does not appear to work in fancy UI
            var bounds = new Rectangle((int)Left.Pixels, (int)Top.Pixels, (int)Width.Pixels, (int)Height.Pixels);
            if(bounds.Contains(Main.mouseX, Main.mouseY) && BuffIconTooltip != "")
            {
                DeckbuildState.SetTooltip(BuffIconTooltip);
            }
        }
    }
}
