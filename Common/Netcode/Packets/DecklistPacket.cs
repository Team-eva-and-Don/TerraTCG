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

	// Packet used to sync each player's ordered decklist with their opponent's client
	// at the start of a multiplayer game
	internal class DecklistPacket : TurnOrderPacket
	{

		private CardCollection Collection { get; set; }


		public DecklistPacket(): base()
		{

		}

		public DecklistPacket(Player player, int opponentId, CardCollection cardCollection): base(
			player, 
			new() { TurnIndex = 255, ActionIndex = 254 }, 
			opponentId)
		{
			Collection = cardCollection;
		}

		protected override void PostReceive(BinaryReader reader, int sender, int recipient, Player player, TurnOrder turnOrder)
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
				GameActionPacketQueue.Instance.QueueOutgoingMessage(
					new DecklistPacket(player, recipient, collection), from: sender);
				new AckPacket(player, turnOrder, recipient).Send(to: sender);
			} else
			{

				// If we are not yet in game - we are receiving an opponent's invite to duel
				// Start the game, then send our deck list back to the opponent
				if (TCGPlayer.LocalGamePlayer == null)
				{
					// Start a game between the player and a networked opponent
					var remotePlayer = NetSyncPlayerSystem.Instance.RegisterPlayer(player.whoAmI, collection);
					ModContent.GetInstance<GameModSystem>().StartGame(remotePlayer, TCGPlayer.LocalPlayer, 0);
					// Send a decklist back to the paired opponent
					var handAndDeck = new CardCollection()
					{
						Cards = [.. TCGPlayer.LocalGamePlayer.Deck.Cards, .. TCGPlayer.LocalGamePlayer.Hand.Cards]
					};
					GameActionPacketQueue.Instance.QueueOutgoingMessage(
						new DecklistPacket(Main.LocalPlayer, player.whoAmI, handAndDeck));

					// Update sync state to let other players know we're no longer looking for a game
					Main.LocalPlayer.GetModPlayer<GameStateSyncPlayer>().BroadcastSyncState();
				} else if (TCGPlayer.LocalGamePlayer.Opponent.Controller is NoOpNetGamePlayerController)
				{
					// Start a game between the player and a networked opponent
					var remotePlayer = NetSyncPlayerSystem.Instance.RegisterPlayer(player.whoAmI, collection);
					// We are already in a game but have not yet replaced the placeholder enemy with the
					// opponent's actual decklist, do that now
					var game = TCGPlayer.LocalGamePlayer.Game;
					game.SwapController(remotePlayer, remotePlayer.Deck.Copy());

					// Update sync state to let other players know we're no longer looking for a game
					Main.LocalPlayer.GetModPlayer<GameStateSyncPlayer>().BroadcastSyncState();
				}
				// Acknowledge the message from the server
				new AckPacket(player, turnOrder, recipient).Send();
			}
		}

		protected override void PostSend(BinaryWriter writer, Player player, TurnOrder turnOrder)
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
