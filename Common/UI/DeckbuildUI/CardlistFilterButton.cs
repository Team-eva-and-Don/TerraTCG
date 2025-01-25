using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.UI.DeckbuildUI
{
    internal class CardlistFilterButton : UIPanel
    {
        internal CardSubtype CardSubtype { get; set; }

        public override void OnInitialize()
        {
            SetPadding(4);
            Width.Pixels = 24;
            Height.Pixels = 24;
            OnMouseOver += (evt, elem) => SoundEngine.PlaySound(SoundID.MenuTick);
            OnMouseOut += (evt, elem) => SoundEngine.PlaySound(SoundID.MenuTick);
        }
        public bool IsEnabled => ((CardListFilter)Parent).FilterTypeEnabled(CardSubtype);

        public override void Update(GameTime gameTime)
        {
            BackgroundColor = IsEnabled ? 
                new Color(73, 94, 171, 180) : new Color(73, 94, 171, 180) * 0.25f;
			BorderColor = IsEnabled ?
				Color.Black : Color.Black * 0.5f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var textureCache = TextureCache.Instance;
            Texture2D texture;
            float scale;
            if(textureCache.BiomeIconBounds.TryGetValue(CardSubtype, out var bounds))
            {
                texture = textureCache.BiomeIcons.Value;
                scale = 0.75f;
            } else if(textureCache.CardTypeEmoteBounds.TryGetValue(CardSubtype, out bounds))
            {
                texture = textureCache.EmoteIcons.Value;
                scale = 1f;
            } else
			{
				texture = textureCache.SpecialCardSubtypes[CardSubtype].Value;
				bounds = texture.Bounds;
				scale = 1f;
			}

            var innerDims = GetInnerDimensions();
            var center = new Vector2(innerDims.X, innerDims.Y) + new Vector2(innerDims.Width, innerDims.Height) / 2;
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;

            var brightness = IsEnabled ? 1f : 0.25f;
            spriteBatch.Draw(texture, center, bounds, Color.White * brightness, 0, origin, scale, SpriteEffects.None, 0);

            if(ContainsPoint(Main.MouseScreen))
            {
                var cardCategory = Language.GetTextValue($"Mods.TerraTCG.Cards.Types.{CardSubtype}");
                var tooltipText = Language.GetTextValue("Mods.TerraTCG.Cards.Common.ShowCards").Replace("%%", cardCategory);
                DeckbuildState.SetTooltip(tooltipText);
            }

        }
    }
}
