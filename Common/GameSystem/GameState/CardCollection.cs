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
            return Cards.Count == 20 && Cards.Where(
				c => c.CardType == CardType.CREATURE && !c.SubTypes.Contains(CardSubtype.EXPERT) && !c.SubTypes.Contains(CardSubtype.CRITTER)
			).Any();
        }

        public List<string> Serialize()
        {
            // Store each card in the collection as "ModName/CardName"
            return Cards.Select(c => c.FullName).ToList();
        }

        public void DeSerialize(List<string> cardFullNames)
        {
            var allCards = ModContent.GetContent<BaseCardTemplate>()
                .Select(t => t.Card);

            // TODO handle errors
            Cards = cardFullNames
				.Where(fn => allCards.Any(c => c.FullName == fn))
                .Select(fn => allCards.Where(c => c.FullName == fn).FirstOrDefault())
                .ToList();
        }
    }
}
