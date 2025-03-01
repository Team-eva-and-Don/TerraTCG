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
	}
	// Sort-able 
	internal abstract class TurnOrderPacket : PlayerPacket
	{
		protected TurnOrder turnOrder;

		public TurnOrderPacket() : base()
		{

		}

		public TurnOrderPacket(Player player) : base(player) 
		{
			var playerGame = player.GetModPlayer<TCGPlayer>().GamePlayer.Game;
			var turnIdx = playerGame.CurrentTurn.TurnCount;
			var actionIdx = playerGame.CurrentTurn.ActionLog.Count;
			turnOrder = new()
			{
				TurnIndex = turnIdx,
				ActionIndex = actionIdx
			};
		}

		public TurnOrderPacket(Player player, TurnOrder turnOrder) : base(player) 
		{ 
			this.turnOrder = turnOrder;
		}
		protected override void PostSend(BinaryWriter writer, Player player)
		{
			writer.Write((byte)turnOrder.TurnIndex);
			writer.Write((byte)turnOrder.ActionIndex);
			PostSend(writer, player, turnOrder);
		}

		protected override void PostReceive(BinaryReader reader, int sender, Player player)
		{
			byte turnIdx = reader.ReadByte();
			byte actionIdx = reader.ReadByte();
			PostReceive(reader, sender, player, new TurnOrder()
			{
				TurnIndex = turnIdx,
				ActionIndex = actionIdx,
			});
		}

		protected abstract void PostSend(BinaryWriter writer, Player player, TurnOrder order);
		protected abstract void PostReceive(BinaryReader reader, int sender, Player player, TurnOrder order);
	}
}
