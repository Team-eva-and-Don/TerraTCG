using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.UI.GameFieldUI;

namespace TerraTCG.Common.UI.DeckbuildUI
{
    internal class PlayerDeckList: UIPanel
    {
        private List<DecklistCardElement> deckList;

        private UIScrollbar scrollBar;

        private const int MAX_SLOTS = 30;

        public override void OnInitialize()
        {
            deckList = [];

            for (int _ = 0; _ < MAX_SLOTS; _++)
            {

                deckList.Add(new DecklistCardElement()
                {
                    PaddingRight = 6f,
                    PaddingLeft = 6f,
                    PaddingTop = 4f,
                    PaddingBottom = 4f
                });
                Append(deckList.Last());
            }

            scrollBar = new UIScrollbar
            {
                HAlign = 1f
            };
            scrollBar.Height.Percent = 1f;
            Append(scrollBar);
        }

        internal int GetMaxRows()
        {
            var heightPerCard = DecklistCardElement.PANEL_HEIGHT;
            var maxWindowHeight = (int)GetInnerDimensions().Height;
            int maxRowCount = maxWindowHeight / heightPerCard;
            return maxRowCount;
        }

        private void UpdateCardPositions()
        {
            var cardList = TCGPlayer.LocalPlayer.Deck.Cards;

            var cardCounts = cardList.GroupBy(g => g.Name)
                .Select(group => (group.First(), group.Count()))
                .OrderBy(pair=>pair.Item1.CardName)
                .ToList();

            var scrollOffset = scrollBar.ViewPosition / 19f; // empirically determined to be [0->20)
            var totalRowCount = cardCounts.Count;

            var visibleRows = GetMaxRows();
            var maxScroll = totalRowCount - visibleRows;

            var topRow = (int)MathHelper.Lerp(0, maxScroll, scrollOffset);
            var yOffset = topRow * DecklistCardElement.PANEL_HEIGHT;

            for (int i = 0; i < cardCounts.Count; i++)
            {
                var card = deckList[i];
                card.SourceCard = cardCounts[i].Item1;
                card.Count = cardCounts[i].Item2;
                GameFieldState.SetRectangle(card, 0, DecklistCardElement.PANEL_HEIGHT * i - yOffset, 260, 56);
            }

            for(int i = cardCounts.Count; i < deckList.Count; i++)
            {
                var card = deckList[i];
                card.SourceCard = null;
                card.Count = 0;
                GameFieldState.SetRectangle(card, 0, DecklistCardElement.PANEL_HEIGHT * i - yOffset, 260, 56);
            }
        }
        public override void Update(GameTime gameTime)
        {
            Main.LocalPlayer.mouseInterface = true;
            UpdateCardPositions();
            base.Update(gameTime);
        }
    }
}
