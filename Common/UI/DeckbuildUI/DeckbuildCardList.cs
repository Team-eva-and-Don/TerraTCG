using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using TerraTCG.Common.GameSystem.CardData;
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
            cards = Assembly.GetAssembly(typeof(BaseCardTemplate))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(BaseCardTemplate)))
                .Select(t => ((BaseCardTemplate)Activator.CreateInstance(t)).CreateCard())
                .Where(c=>c.IsCollectable)
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

            for(int i = 0; i < cards.Count; i++)
            {
                int row = i / CARDS_PER_ROW;
                int col = i % CARDS_PER_ROW;
                GameFieldState.SetRectangle(cards[i],
                    col * (CARD_WIDTH * CARD_SCALE + CARD_MARGIN),
                    row * (CARD_HEIGHT * CARD_SCALE + CARD_MARGIN) - yOffset,
                    CARD_WIDTH * CARD_SCALE,
                    CARD_HEIGHT * CARD_SCALE);
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
