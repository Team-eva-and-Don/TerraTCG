using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem;
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
			}



			if(Main.netMode == NetmodeID.Server)
			{
				// ModContent.GetInstance<GameModSystem>().StartGame(remotePlayer, new NoOpNetGamePlayerController());
				new DecklistPacket(player, collection).Send(from: sender);
			} else
			{
				// Start a game between the player and a networked opponent
				var remotePlayer = NetSyncPlayerSystem.Instance.RegisterPlayer(player.whoAmI, collection);
				// If we are not yet in game - we are receiving an opponent's invite to duel
				// Start the game, then send our deck list back to the opponent
				if (TCGPlayer.LocalGamePlayer == null)
				{
					ModContent.GetInstance<GameModSystem>().StartGame(remotePlayer, TCGPlayer.LocalPlayer, 0);
					// Send a decklist back to the paired opponent
					var handAndDeck = new CardCollection()
					{
						Cards = [.. TCGPlayer.LocalGamePlayer.Deck.Cards, .. TCGPlayer.LocalGamePlayer.Hand.Cards]
					};
					new DecklistPacket(Main.LocalPlayer, handAndDeck).Send(to: sender);
				} else
				{
					// We are already in a game - and are receiving our opponent's decklist. Update the
					// state of our opponent
					var game = TCGPlayer.LocalGamePlayer.Game;
					game.SwapController(remotePlayer, remotePlayer.Deck.Copy());
				}

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
