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
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.UI.Common;
using TerraTCG.Common.UI.DeckbuildUI;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class CardPreviewElement : DraggableUIElement
    {
        internal const float CARD_SCALE = 4f / 3f;
        private string BuffIconTooltip;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var localPlayer = TCGPlayer.LocalPlayer;
            if (localPlayer == null || localPlayer.MouseoverCard == null)
            {
                return;
            }
            var fadePoint = localPlayer.GamePlayer == null ? 1f : TCGPlayer.FieldTransitionPoint;
            var card = localPlayer.MouseoverCard;
            var texture = card.Texture;
            var attackOverride = localPlayer.MouseoverZone?.PlacedCard?.GetAttackWithModifiers(localPlayer.MouseoverZone, null);
            spriteBatch.Draw(texture.Value, Position, texture.Value.Bounds, Color.White * fadePoint, 0, default, CARD_SCALE, SpriteEffects.None, 0f);
            CardTextRenderer.Instance.DrawCardText(
                spriteBatch, card, Position, CARD_SCALE, attackOverride: attackOverride);

            if(attackOverride != null)
            {
                var center = Position + new Vector2(2, texture.Value.Height * CARD_SCALE);
                foreach(var modifier in localPlayer.MouseoverZone.PlacedCard.GetKeywordModifiers().Keys.Order())
                {
                    var iconTexture = TextureCache.Instance.ModifierIconTextures[modifier].Value;
                    var origin = new Vector2(0, iconTexture.Height);
                    spriteBatch.Draw(iconTexture, center, iconTexture.Bounds, Color.White, 0, origin, 1, SpriteEffects.None, 0);
                    center.X += iconTexture.Width + 2;
                }
            }

            //if(BuffIconTooltip != null)
            //{
            //    var buffIconPos = Main.MouseScreen + new Vector2(16, 16);
            //    var font = FontAssets.MouseText.Value;
            //    CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, BuffIconTooltip, buffIconPos, font: font);
            //}

            // This elem is used in both the fancy and normal UI, regular "ContainsPoint"
            // does not appear to work in fancy UI
            var bounds = new Rectangle((int)Left.Pixels, (int)Top.Pixels, (int)Width.Pixels, (int)Height.Pixels);
            if(bounds.Contains(Main.mouseX, Main.mouseY))
            {
                DeckbuildState.SetTooltip("Hovering card!");
            }
        }
    }
}
