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
using TerraTCG.Common.UI.Common;

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

        private UIBetterTextBox searchTextBox;
        internal List<CardSubtype> VisibleTypes = [];

        public string FilterString { get; private set; }

        public override void OnInitialize()
        {
            filterButtons = [];
            searchTextBox = new(Language.GetText("Mods.TerraTCG.Cards.Common.Search"))
            {
                BackgroundColor = Color.White,
                Top = {Percent = 0.45f},
                Left = {Percent = 0f}, 
                Width = {Pixels=120f},
                Height = {Pixels=30f},
            };
            searchTextBox.OnTextChanged += SearchTextBox_OnTextChanged;
            Append(searchTextBox);

            for(int i = 0; i < FilterTypes.Count; i++)
            {
                var filterType = FilterTypes[i];
                var btn = new CardlistFilterButton()
                {
                    CardSubtype = filterType,
                };
                btn.Top.Percent = 0.5f;
                btn.Left.Percent = 0.21f + 0.8f * (i / (float)FilterTypes.Count);
                btn.OnLeftClick += (evt, elem) => ToggleVisibility(evt, elem, filterType);

                Append(btn);
                filterButtons.Add(btn);
            }
        }

        private void SearchTextBox_OnTextChanged()
        {
            FilterString = searchTextBox.currentString;
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
