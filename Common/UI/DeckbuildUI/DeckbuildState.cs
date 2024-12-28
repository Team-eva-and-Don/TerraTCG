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

        const int DECKBUILD_WIDTH = 640;
        public override void OnInitialize()
        {
            base.OnInitialize();
            deckbuildCardList = new()
            {
                BackgroundColor = new Color(73, 94, 171, 180),
                PaddingLeft = 8f,
                PaddingRight = 8f,
                PaddingTop = 16f,
                PaddingBottom = 16f,
            };

            var scrollBar = new UIScrollbar
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
            for(int i = 0; i < cards.Count; i++)
            {
                int row = i / CARDS_PER_ROW;
                int col = i % CARDS_PER_ROW;
                GameFieldState.SetRectangle(cards[i],
                    col * (CARD_WIDTH * CARD_SCALE + CARD_MARGIN),
                    row * (CARD_HEIGHT * CARD_SCALE + CARD_MARGIN),
                    CARD_WIDTH * CARD_SCALE,
                    CARD_HEIGHT * CARD_SCALE);
            }
        }

        public override void Update(GameTime gameTime)
        {
            GameFieldState.SetRectangle(deckbuildCardList, (Main.screenWidth - DECKBUILD_WIDTH) / 2, 96, DECKBUILD_WIDTH, Main.screenHeight - 2 * 96);
            GameFieldState.SetRectangle(cancelButton, 16, Main.screenHeight - 16 - 48, 38, 48);
            CalculateCardPositions();
            base.Update(gameTime);
        }
    }
}
