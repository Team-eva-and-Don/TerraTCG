using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem;

namespace TerraTCG.Common.Netcode.Packets
{
	internal struct TurnOrder
	{
		public int TurnIndex { get; set; }
		public int ActionIndex { get; set; }

		public readonly bool Equals(TurnOrder other)
		{
			return TurnIndex == other.TurnIndex && ActionIndex == other.ActionIndex;
		}
	}
	// Sort-able 
	internal abstract class TurnOrderPacket : PlayerPacket
	{
		internal TurnOrder TurnOrder { get; set; }

		internal int OpponentId { get; set; }

		public TurnOrderPacket() : base()
		{

		}

		public TurnOrderPacket(Player player) : base(player) 
		{
			var playerGame = player.GetModPlayer<TCGPlayer>().GamePlayer.Game;
			var turnIdx = playerGame.CurrentTurn.TurnCount;
			var actionIdx = playerGame.CurrentTurn.ActionLog.Count;
			TurnOrder = new()
			{
				TurnIndex = turnIdx,
				ActionIndex = actionIdx
			};
			OpponentId = player.GetModPlayer<TCGPlayer>().GamePlayer.OpponentPlayerId;
		}

		public TurnOrderPacket(Player player, TurnOrder turnOrder, int opponentId) : base(player) 
		{ 
			TurnOrder = turnOrder;
			OpponentId = opponentId;
		}
		protected override void PostSend(BinaryWriter writer, Player player)
		{
			writer.Write((byte)TurnOrder.TurnIndex);
			writer.Write((byte)TurnOrder.ActionIndex);
			writer.Write((byte)OpponentId);
			PostSend(writer, player, TurnOrder);
		}

		protected override void PostReceive(BinaryReader reader, int sender, Player player)
		{
			byte turnIdx = reader.ReadByte();
			byte actionIdx = reader.ReadByte();
			byte opponentId = reader.ReadByte();
			PostReceive(reader, sender, opponentId, player, new TurnOrder()
			{
				TurnIndex = turnIdx,
				ActionIndex = actionIdx,
			});
		}

		protected abstract void PostSend(BinaryWriter writer, Player player, TurnOrder order);
		protected abstract void PostReceive(BinaryReader reader, int sender, int recipient, Player player, TurnOrder order);
	}
}
