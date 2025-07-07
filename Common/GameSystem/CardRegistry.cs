using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem
{

	public  class CardRegistry : ModSystem
	{
		public static CardRegistry Instance;

		public static Action<Card> CardChanged;

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

		public static void AddCard(Card card)
		{
			_allCards.Add(card);
		}

		public static void AddCard(params Card[] cards)
		{
			_allCards.AddRange(cards);
		}
	}
}
