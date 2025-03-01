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

		public PassTurnPacket(Player player, TurnOrder turnOrder) : base(player, turnOrder) { }

		protected override void PostReceive(BinaryReader reader, int sender, Player player, TurnOrder turnOrder)
		{

			if(Main.netMode == NetmodeID.Server)
			{
				new PassTurnPacket(player, turnOrder).Send(from: sender);
			} else
			{
				var remotePlayer = NetSyncPlayerSystem.Instance.SyncPlayerMap[player.whoAmI];
				remotePlayer.PassTurn();
			}
		}

		protected override void PostSend(BinaryWriter writer, Player player, TurnOrder turnOrder)
		{
			// No-op
		}
	}
}
