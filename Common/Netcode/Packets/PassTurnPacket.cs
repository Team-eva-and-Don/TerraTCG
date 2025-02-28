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
	internal class PassTurnPacket : PlayerPacket
	{
		public PassTurnPacket() : base() { }

		public PassTurnPacket(Player player) : base(player) { }

		protected override void PostReceive(BinaryReader reader, int sender, Player player)
		{

			if(Main.netMode == NetmodeID.Server)
			{
				new PassTurnPacket(player).Send(from: sender);
			} else
			{
				var remotePlayer = NetSyncPlayerSystem.Instance.SyncPlayerMap[player.whoAmI];
				remotePlayer.PassTurn();
			}
		}

		protected override void PostSend(BinaryWriter writer, Player player)
		{
			// No-op
		}
	}
}
