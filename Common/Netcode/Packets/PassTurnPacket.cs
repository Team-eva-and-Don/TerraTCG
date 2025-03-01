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
	internal class PassTurnPacket : TurnOrderPacket
	{
		public PassTurnPacket() : base() { }

		public PassTurnPacket(Player player) : base(player) { }

		public PassTurnPacket(Player player, TurnOrder turnOrder, int opponentId) : base(player, turnOrder, opponentId) { }

		protected override void PostReceive(BinaryReader reader, int sender, int recipient, Player player, TurnOrder turnOrder)
		{

			if(Main.netMode == NetmodeID.Server)
			{
				GameActionPacketQueue.Instance.QueueOutgoingMessage(new PassTurnPacket(player, turnOrder, recipient), from: sender);

				new AckPacket(player, turnOrder, recipient).Send(to: sender);
			} else
			{
				var remotePlayer = NetSyncPlayerSystem.Instance.SyncPlayerMap[player.whoAmI];
				remotePlayer.PassTurn(turnOrder);
				new AckPacket(player, turnOrder, recipient).Send();
			}
		}

		protected override void PostSend(BinaryWriter writer, Player player, TurnOrder turnOrder)
		{
			// No-op
		}
	}
}
