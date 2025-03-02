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
	// Action packet used to send the "surrender" button click to the other client
	// in a multiplayer match
	internal class SurrenderPacket : TurnOrderPacket
	{
		public SurrenderPacket() : base() { }

		public SurrenderPacket(Player player) : base(player) 
		{
			// Use special index for surrender action to ensure it doesn't
			// collide with another game action
			TurnOrder = new() { ActionIndex = 255, TurnIndex = 255 };
		}

		public SurrenderPacket(Player player, TurnOrder turnOrder, int opponentId) : base(player, turnOrder, opponentId) { }

		protected override void PostReceive(BinaryReader reader, int sender, int recipient, Player player, TurnOrder turnOrder)
		{

			if(Main.netMode == NetmodeID.Server)
			{
				GameActionPacketQueue.Instance.QueueOutgoingMessage(new SurrenderPacket(player, turnOrder, recipient), from: sender);

				new AckPacket(player, turnOrder, recipient).Send(to: sender);
			} else
			{
				var remotePlayer = NetSyncPlayerSystem.Instance.SyncPlayerMap[player.whoAmI];
				remotePlayer.Surrender(turnOrder);
				new AckPacket(player, turnOrder, recipient).Send();
			}
		}

		protected override void PostSend(BinaryWriter writer, Player player, TurnOrder turnOrder)
		{
			// No-op
		}
	}
}
