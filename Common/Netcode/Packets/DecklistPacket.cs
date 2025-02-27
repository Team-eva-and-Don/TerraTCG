using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.Netcode.Packets
{
	internal class CardNetworkSync : ModSystem
	{
		public static CardNetworkSync Instance => ModContent.GetInstance<CardNetworkSync>();
		// TODO *really* need a central location for this
		private List<Card> _allCards;
		public List<Card> AllCards
		{
			get
			{
				_allCards ??= ModContent.GetContent<BaseCardTemplate>()
					.Select(t => t.Card)
					.ToList();
				return _allCards;
			}
		}

		public static ushort Serialize(Card card) => (ushort)Instance.AllCards.IndexOf(card);

		public static Card Deserialize(ushort idx) => Instance.AllCards[idx];
	}

	internal class DecklistPacket : PlayerPacket
	{

		private CardCollection Collection { get; set; }


		public DecklistPacket(): base()
		{

		}

		public DecklistPacket(Player player, CardCollection cardCollection): base(player)
		{
			Collection = cardCollection;
		}

		protected override void PostReceive(BinaryReader reader, int sender, Player player)
		{
			var cardCount = reader.ReadInt16();
			var collection = new CardCollection();
			for(int _ = 0; _ < cardCount; _++)
			{
				var cardIdx = reader.ReadUInt16();
				var card = CardNetworkSync.Deserialize(cardIdx);
				collection.Add(card);
				Main.NewText(card.Name);
			}

			if(Main.netMode == NetmodeID.Server)
			{
				new DecklistPacket(player, collection).Send(from: sender);
			}
		}

		protected override void PostSend(BinaryWriter writer, Player player)
		{
			// Write the size of the collection, then each card one by one
			writer.Write((short)Collection.Cards.Count);
			foreach (var card in Collection.Cards)
			{
				writer.Write(CardNetworkSync.Serialize(card));
			}
		}
	}
}
