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
using TerraTCG.Common.UI.GameFieldUI;
using static TerraTCG.Common.UI.DeckbuildUI.DeckbuildCardElement;

namespace TerraTCG.Common.UI.DeckbuildUI
{
    internal class DeckbuildState : UIState
    {
        internal CancelResumeGameButton cancelButton;
        internal List<DeckbuildCardElement> cards;
        private DeckbuildCardList deckbuildCardList;
        private UIScrollbar scrollBar;

        const int DECKBUILD_WIDTH = 640;
        const int MIN_Y_MARGIN = 32;
        const int PADDING_Y = 16;
        public override void OnInitialize()
        {
            base.OnInitialize();
            deckbuildCardList = new()
            {
                BackgroundColor = new Color(73, 94, 171, 180),
                PaddingLeft = 8f,
                PaddingRight = 8f,
                PaddingTop = PADDING_Y,
                PaddingBottom = PADDING_Y,
            };

            scrollBar = new UIScrollbar
            {
                HAlign = 1f
            };
            scrollBar.Height.Percent = 1f;

            cancelButton = new()
            {
                OnClickAction = ()=>ModContent.GetInstance<UserInterfaces>().StopDeckbuild()
            };
            Append(cancelButton);

            Append(deckbuildCardList);
            deckbuildCardList.Append(scrollBar);

            cards = Assembly.GetAssembly(typeof(BaseCardTemplate))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(BaseCardTemplate)))
                .OrderBy(t => t.Name)
                .Select(t => new DeckbuildCardElement(((BaseCardTemplate)Activator.CreateInstance(t)).CreateCard()))
                .ToList();

            foreach (var card in cards)
            {
                deckbuildCardList.Append(card);
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

            var visibleRows = GetWindowHeight().Item1;
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

        // For ease of calculating scroll, make the window height an integer number of cards
        private (int, int) GetWindowHeight()
        {
            var heightPerCard = (int)(CARD_HEIGHT * CARD_SCALE + CARD_MARGIN);
            var maxWindowHeight = Main.screenHeight - 2 * MIN_Y_MARGIN - 2* PADDING_Y;
            int maxRowCount = maxWindowHeight / heightPerCard;
            return (maxRowCount, maxRowCount * heightPerCard + 2 * PADDING_Y);
        }

        public override void Update(GameTime gameTime)
        {
            var windowHeight = GetWindowHeight().Item2;
            GameFieldState.SetRectangle(deckbuildCardList, 
                (Main.screenWidth - DECKBUILD_WIDTH) / 2, 
                (Main.screenHeight - windowHeight) / 2, DECKBUILD_WIDTH, windowHeight);
            GameFieldState.SetRectangle(cancelButton, 16, Main.screenHeight - 16 - 48, 38, 48);
            CalculateCardPositions();
            base.Update(gameTime);
        }
    }
}
