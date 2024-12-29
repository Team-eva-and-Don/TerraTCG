using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState
{
    // Class representing an ordered collection of cards - eg. a hand, deck, or graveyard
    internal class CardCollection
    {
        internal List<Card> Cards { get; set; } = [];
        private readonly Random random = new();


        public void Shuffle()
        {
            // TODO
            Cards = [.. Cards.OrderBy(c => random.NextInt64())];
        }

        public Card Draw()
        {
            var toReturn = Cards.Last();
            Cards.RemoveAt(Cards.Count - 1);
            return toReturn;
        }

        public void Add(Card card)
        {
            Cards.Add(card);
        } 

        public void Remove(Card card)
        {
            Cards.Remove(card);
        }

        public bool Empty()
        {
            return Cards.Count == 0;
        }

        public CardCollection Copy()
        {
            return new CardCollection()
            {
                Cards = new List<Card>(Cards)
            };
        }
    }
}
