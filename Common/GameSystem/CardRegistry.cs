using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.PackOpening;

namespace TerraTCG.Common.GameSystem
{

	public class CardRegistry : ModSystem
	{
		public static CardRegistry Instance;

		public static event Action OnCardsAdded;

		public override void Load()
		{
			Instance = this;
		}

		private static List<Card> _allCards;
		public static List<Card> AllCards
		{
			get
			{
				_allCards ??= ModContent.GetContent<BaseCardTemplate>()
							.Select(t => t.Card)
							.ToList();
				return _allCards;
			}
		}


		public static void AddCards(params Card[] cards)
		{
			AllCards.AddRange(cards);
			foreach (Card card in cards)
				if (card.IsCollectable)
					CardPools.CollectableCards.Add(card);
			OnCardsAdded?.Invoke();
		}
	}
}
