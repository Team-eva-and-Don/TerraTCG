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
        internal List<Card> Cards { get; set; } = new List<Card>();


        public void Shuffle()
        {
            // TODO
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
    }
}
