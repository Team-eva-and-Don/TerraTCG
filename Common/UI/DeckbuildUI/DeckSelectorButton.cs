using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;

namespace TerraTCG.Common.UI.DeckbuildUI
{
    internal class DeckSelectorButton : UIPanel
    {
        internal int DeckIdx { get; set; }
        internal string Label => $"{DeckIdx+1}";

        public override void OnInitialize()
        {
            SetPadding(4);
            Width.Pixels = 24;
            Height.Pixels = 24;

            OnMouseOver += (evt, elem) => SoundEngine.PlaySound(SoundID.MenuTick);
            OnMouseOut += (evt, elem) => SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            BackgroundColor = TCGPlayer.LocalPlayer.ActiveDeck == DeckIdx ? 
                Color.Goldenrod * 0.75f : new Color(73, 94, 171, 180);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(Label == null)
            {
                return;
            }
            base.Draw(spriteBatch);

            var innerDims = GetInnerDimensions();
            var center = new Vector2(innerDims.X, innerDims.Y) + new Vector2(innerDims.Width, innerDims.Height) / 2;

            var font = FontAssets.MouseText.Value;
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, Label, center + 4 * Vector2.UnitY, centered: true, font: font);

            if(ContainsPoint(Main.MouseScreen))
            {
                var tooltipText = Language.GetTextValue("Mods.TerraTCG.Cards.Common.UseDeck").Replace("%%", Label);
                DeckbuildState.SetTooltip(tooltipText);
            }

        }
    }
}
