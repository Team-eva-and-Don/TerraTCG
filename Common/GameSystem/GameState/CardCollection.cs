using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerraTCG.Common.GameSystem.CardData;

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

        public List<string> Serialize()
        {
            return Cards.Select(c => c.Name).ToList();

        }

        public void DeSerialize(List<string> cardNames)
        {
            var allCards = ModContent.GetContent<BaseCardTemplate>()
                .Select(t => t.Card)
                .ToDictionary(c => c.Name, c => c);

            // TODO handle errors rather than silently discarding
            Cards = cardNames
                .Where(allCards.ContainsKey)
                .Select(c => allCards[c])
                .ToList();
        }
    }
}
