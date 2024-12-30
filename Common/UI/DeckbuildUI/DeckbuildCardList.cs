using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI.GameFieldUI;
using static TerraTCG.Common.UI.DeckbuildUI.DeckbuildCardElement;

namespace TerraTCG.Common.UI.DeckbuildUI
{
    internal class DeckbuildCardList : UIPanel
    {
        internal List<DeckbuildCardElement> cards;
        private UIScrollbar scrollBar;

        public override void OnInitialize()
        {
            base.OnInitialize();
            cards = ModContent.GetContent<BaseCardTemplate>()
                .Select(t=>t.Card)
                .Where(c => c.IsCollectable)
                .OrderBy(t => t.SortType)
                .ThenBy(t => t.Name)
                .Select(c => new DeckbuildCardElement(c))
                .ToList();

            foreach (var card in cards)
            {
                Append(card);
            }
            scrollBar = new UIScrollbar
            {
                HAlign = 1f
            };
            scrollBar.Height.Percent = 1f;
            Append(scrollBar);
        }

        private List<DeckbuildCardElement> GetVisibleCards()
        {
            // TODO is this the best way to pass data between sibling elements
            var cardFilter = ((DeckbuildState)Parent).VisibleCardTypes;
            if(cardFilter.Count == 0)
            {
                return cards;
            } else
            {
                return cards.Where(c => cardFilter.Contains(c.SourceCard.SortType)).ToList();
            }
        }
        private void CalculateCardPositions()
        {
            int CARDS_PER_ROW = 5;
            var scrollOffset = scrollBar.ViewPosition / 19f; // empirically determined to be [0->20)
            var totalRowCount = (cards.Count / CARDS_PER_ROW);
            if(cards.Count % CARDS_PER_ROW != 0)
            {
                totalRowCount += 1;
            }

            var visibleRows = DeckbuildState.GetWindowHeight().Item1;
            var maxScroll = totalRowCount - visibleRows;

            var topRow = (int)MathHelper.Lerp(0, maxScroll, scrollOffset);
            var yOffset = topRow * (int)(CARD_HEIGHT * CARD_SCALE + CARD_MARGIN);

            var visibleCards = GetVisibleCards();
            for(int i = 0; i < visibleCards.Count; i++)
            {
                int row = i / CARDS_PER_ROW;
                int col = i % CARDS_PER_ROW;
                GameFieldState.SetRectangle(visibleCards[i],
                    col * (CARD_WIDTH * CARD_SCALE + CARD_MARGIN),
                    row * (CARD_HEIGHT * CARD_SCALE + CARD_MARGIN) - yOffset,
                    CARD_WIDTH * CARD_SCALE,
                    CARD_HEIGHT * CARD_SCALE);
            }
            var invisibleCards = cards.Except(visibleCards);
            // Push the non-visible cards off the bottom of the screen
            // so that their own bounds-checking code hides them
            foreach(var card in invisibleCards)
            {
                GameFieldState.SetRectangle(card, 0, Height.Pixels + 5);
            }

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            CalculateCardPositions();
        }
    }
}
