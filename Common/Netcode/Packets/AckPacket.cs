using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace TerraTCG.Common.Netcode.Packets
{
	internal class AckPacket : TurnOrderPacket
	{
		public AckPacket() : base() { }

		public AckPacket(Player player) : base(player) { }

		public AckPacket(Player player, TurnOrder turnOrder) : base(player, turnOrder) { }

		protected override void PostReceive(BinaryReader reader, int sender, Player player, TurnOrder turnOrder)
		{
			// Server doesn't broadcast acks to other clients
			GameActionPacketQueue.Instance.DeuqueueOutgoingMessage(player, turnOrder);
		}

		protected override void PostSend(BinaryWriter writer, Player player, TurnOrder turnOrder)
		{
			// No-op
		}
	}
}
