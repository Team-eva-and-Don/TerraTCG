using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.UI.DeckbuildUI
{
    internal class CardListFilter : UIPanel
    {
        private List<CardSubtype> FilterTypes = [
            // Biomes
            CardSubtype.FOREST,
            CardSubtype.CAVERN,
            CardSubtype.JUNGLE,
            CardSubtype.GOBLIN_ARMY,
            CardSubtype.BLOOD_MOON,
            CardSubtype.OCEAN,
            CardSubtype.MUSHROOM,
            // Card types
            CardSubtype.EQUIPMENT,
            CardSubtype.CONSUMABLE,
            CardSubtype.TOWNSFOLK,
        ];

        private List<CardlistFilterButton> filterButtons;

        internal List<CardSubtype> VisibleTypes = [];

        public override void OnInitialize()
        {
            filterButtons = [];
            for(int i = 0; i < FilterTypes.Count; i++)
            {
                var filterType = FilterTypes[i];
                var btn = new CardlistFilterButton()
                {
                    CardSubtype = filterType,
                };
                btn.Top.Percent = 0.5f;
                btn.Left.Percent = i / (float)FilterTypes.Count;
                btn.OnLeftClick += (evt, elem) => ToggleVisibility(evt, elem, filterType);

                Append(btn);
                filterButtons.Add(btn);
            }
        }

        private void ToggleVisibility(UIMouseEvent evt, UIElement listeningElement, CardSubtype filterType)
        {
            if (FilterTypeEnabled(filterType))
            {
                VisibleTypes.Remove(filterType);
            }
            else
            {
                VisibleTypes.Add(filterType);
            }
        }

        internal bool FilterTypeEnabled(CardSubtype filterType) => VisibleTypes.Contains(filterType);

        public override void Update(GameTime gameTime)
        {
            Main.LocalPlayer.mouseInterface = true;
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var textPos = new Vector2(GetInnerDimensions().X, GetInnerDimensions().Y);
            var font = FontAssets.MouseText.Value;
            var text = Language.GetTextValue("Mods.TerraTCG.Cards.Common.FilterCards");
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, text, textPos, font: font);
        }
    }
}
