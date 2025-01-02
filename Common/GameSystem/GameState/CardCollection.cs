using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerraTCG.Common.GameSystem.CardData;

namespace TerraTCG.Common.GameSystem.GameState
{
    // Class representing an ordered collection of cards - eg. a hand, deck, or graveyard
    internal class CardCollection
    {
        internal List<Card> Cards { get; set; } = [];

        public void Shuffle()
        {
            // TODO
            Cards = [.. Cards.OrderBy(c => Main.rand.Next())];
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

        public bool ValidateDeck()
        {
            return Cards.Count == 20 && Cards.Where(c => c.CardType == CardType.CREATURE && !c.SubTypes.Contains(CardSubtype.EXPERT)).Any();
        }

        public List<uint> Serialize()
        {
            return Cards.GroupBy(c => c.ID)
                .SelectMany(groups => new List<uint>() { groups.First().ID, (uint)groups.Count() })
                .ToList();

        }

        public void DeSerialize(List<uint> cardCounts)
        {
            var allCards = ModContent.GetContent<BaseCardTemplate>()
                .Select(t => t.Card)
                .ToDictionary(c => c.ID, c => c);

            // TODO handle errors
            Cards = [];
            for(int i = 0; i < cardCounts.Count; i+=2)
            {
                uint cardId = cardCounts[i];
                uint count = cardCounts[i + 1];
                var card = allCards[cardId];
                for(int _ = 0; _ < count; _++)
                {
                    Cards.Add(card);
                }
            }
        }
    }
}
