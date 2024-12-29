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
        internal List<DecklistCardElement> deckList;

        public override void OnInitialize()
        {
            deckList = [];

            for (int _ = 0; _ < 20; _++)
            {

                deckList.Add(new DecklistCardElement()
                {
                    PaddingRight = 8f,
                    PaddingLeft = 8f,
                    PaddingTop = 4f,
                    PaddingBottom = 4f
                });
                Append(deckList.Last());
            }

        }

        private void UpdateCardPositions()
        {
            var cardList = Main.LocalPlayer.GetModPlayer<TCGPlayer>().Deck.Cards;

            var cardCounts = cardList.GroupBy(g => g.Name).Select(y => (y.First(), y.Count())).ToList();
            for (int i = 0; i < cardCounts.Count; i++)
            {
                var card = deckList[i];
                card.SourceCard = cardCounts[i].Item1;
                card.Count = cardCounts[i].Item2;
                GameFieldState.SetRectangle(card, 0, 64 * i, 260, 60);
            }

            for(int i = cardCounts.Count; i < deckList.Count; i++)
            {
                var card = deckList[i];
                card.SourceCard = null;
                card.Count = 0;
                GameFieldState.SetRectangle(card, 0, 64 * i, 260, 60);
            }
        }
        public override void Update(GameTime gameTime)
        {
            UpdateCardPositions();
            base.Update(gameTime);
        }
    }
}
