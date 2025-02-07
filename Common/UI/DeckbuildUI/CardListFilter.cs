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
using Terraria.ModLoader;
using Terraria.UI;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI.Common;

namespace TerraTCG.Common.UI.DeckbuildUI
{

    internal interface IHasCard
    {
        public Card Card { get; }
    }

    internal class CardListFilter : UIPanel
    {
        private List<CardSubtype> FilterTypes = [
			// Special toggle for owned/unowned cards
            CardSubtype.OWNED,
            // Biomes
            CardSubtype.FOREST,
            CardSubtype.CAVERN,
            CardSubtype.JUNGLE,
            CardSubtype.GOBLIN_ARMY,
            CardSubtype.BLOOD_MOON,
            CardSubtype.OCEAN,
            CardSubtype.MUSHROOM,
            CardSubtype.SNOW,
            CardSubtype.EVIL,
            // Card types
            CardSubtype.EQUIPMENT,
            CardSubtype.CONSUMABLE,
            CardSubtype.TOWNSFOLK,
        ];

        private List<CardlistFilterButton> filterButtons;

        private UIBetterTextBox searchTextBox;
        internal List<CardSubtype> VisibleTypes = [CardSubtype.OWNED];

        public string FilterString { get; private set; }

        private List<Card> allCards;

        private int visibleCount;
        private int totalCount;

		private readonly Dictionary<string, LocalizedText> SearchAliases = new()
		{
			["eoc"] = Language.GetText("NPCName.EyeofCthulhu"),
			["eow"] = Language.GetText("NPCName.EaterofWorldsHead"),
			["boc"] = Language.GetText("NPCName.BrainofCthulhu"),
			["wof"] = Language.GetText("NPCName.WallofFlesh"),
		};

        public override void OnInitialize()
        {
            allCards = ModContent.GetContent<BaseCardTemplate>()
                .Select(t=>t.Card)
                .Where(c => c.IsCollectable)
                .ToList();

            filterButtons = [];
            searchTextBox = new(Language.GetText("Mods.TerraTCG.Cards.Common.Search"))
            {
                BackgroundColor = Color.White,
                Top = {Percent = 0.45f},
                Left = {Percent = 0f}, 
                Width = {Pixels = 120f},
                Height = {Pixels = 30f},
            };
            searchTextBox.OnTextChanged += SearchTextBox_OnTextChanged;
			searchTextBox.OnRightClick += SearchTextBox_OnRightClick;
            Append(searchTextBox);

            for(int i = 0; i < FilterTypes.Count; i++)
            {
                var filterType = FilterTypes[i];
                var btn = new CardlistFilterButton()
                {
                    CardSubtype = filterType,
                };
                btn.Top.Percent = 0.5f;
                btn.Left.Percent = 0.21f + 0.8f * (i / (float)(FilterTypes.Count + 1));
                btn.OnLeftClick += (evt, elem) => ToggleVisibility(evt, elem, filterType);

                Append(btn);
                filterButtons.Add(btn);
            }
        }

		private void SearchTextBox_OnRightClick(UIMouseEvent evt, UIElement listeningElement)
		{
			// Clear out filter string on search text box right click
			FilterString = "";
			searchTextBox.currentString = "";
		}

		private void SearchTextBox_OnTextChanged()
        {
            FilterString = searchTextBox.currentString;
			if(SearchAliases.TryGetValue(FilterString, out var text))
			{
				FilterString = text.Value;
			}
        }

		private bool EvaluateCardOwnershipVisibility(Card card)
		{
			var localPlayer = TCGPlayer.LocalPlayer;
			if (localPlayer.DebugDeckbuildMode)
			{
				return true;
			} else if (VisibleTypes.Contains(CardSubtype.OWNED))
			{
				return localPlayer.Collection.Cards.Contains(card);
			} else
			{
				return localPlayer.Collection.Cards.Contains(card) || localPlayer.SeenCollection.Cards.Contains(card);
			}
		}

        internal IEnumerable<T> FilterCardContainer<T>(IEnumerable<T> cardList) where T: IHasCard
        {
            return cardList
				.Where(c=>EvaluateCardOwnershipVisibility(c.Card))
                .Where(c => VisibleTypes.Count == 0 || (VisibleTypes.Count == 1 && VisibleTypes[0] == CardSubtype.OWNED) || VisibleTypes.Contains(c.Card.SortType))
                .Where(c => FilterString == null || FilterString == "" || c.Card.MatchesTextFilter(FilterString));
        }

        internal IEnumerable<Card> FilterCards(List<Card> cardList)
        {
            return cardList
				.Where(EvaluateCardOwnershipVisibility)
                .Where(c => VisibleTypes.Count == 0 || (VisibleTypes.Count == 1 && VisibleTypes[0] == CardSubtype.OWNED) || VisibleTypes.Contains(c.SortType))
                .Where(c => FilterString == null || FilterString == "" || c.MatchesTextFilter(FilterString));
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
            totalCount = 2 * allCards.Count;
			visibleCount = TCGPlayer.LocalPlayer.Collection.Cards.Count;
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var textPosLeft = new Vector2(GetInnerDimensions().X, GetInnerDimensions().Y);
            var font = FontAssets.MouseText.Value;
            var text = Language.GetTextValue("Mods.TerraTCG.Cards.Common.FilterCards");
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, text, textPosLeft, font: font);

            var textPosRight = textPosLeft + new Vector2(GetInnerDimensions().Width, 0);
            var countText = $"{visibleCount}/{totalCount}";
            var countWidth = font.MeasureString(countText).X;
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, countText, textPosRight - new Vector2(countWidth, 0), font: font);
        }
    }
}
