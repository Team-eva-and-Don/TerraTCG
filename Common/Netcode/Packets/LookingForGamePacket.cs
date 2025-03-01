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
	internal class LookingForGamePacket : PlayerPacket
	{
		public LookingForGamePacket() : base() { }
		public LookingForGamePacket(Player player) : base(player) { }
		protected override void PostReceive(BinaryReader reader, int sender, Player player)
		{
			// TODO does server need to keep track of pending games or just clients?
			if(Main.netMode == NetmodeID.Server)
			{
				new LookingForGamePacket(player).Send(from: sender);
			} else
			{
				MatchmakingSystem.Instance.AddLookingForGamePlayer(player);
				Main.NewText($"{player.name} is looking for a game of TerraTCG!");
			}
		}

		protected override void PostSend(BinaryWriter writer, Player player)
		{
			// No-op
		}
	}
}
