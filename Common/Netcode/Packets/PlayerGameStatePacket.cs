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
	internal class PlayerGameStatePacket : PlayerPacket
	{
		private bool inGame;

		private bool lookingForGame;

		private TurnOrder turnOrder;

		public PlayerGameStatePacket() : base()
		{

		}

		public PlayerGameStatePacket(Player player) : base(player)
		{
			var syncPlayer = player.GetModPlayer<GameStateSyncPlayer>();
			inGame = syncPlayer.InGame;
			lookingForGame = syncPlayer.LookingForGame;
			turnOrder = syncPlayer.TurnOrder;
		}

		public PlayerGameStatePacket(Player player, bool inGame, bool lookingForGame, TurnOrder turnOrder) : base(player)
		{
			this.inGame = inGame;
			this.lookingForGame = lookingForGame;
			this.turnOrder = turnOrder;
		}

		protected override void PostReceive(BinaryReader reader, int sender, Player player)
		{
			var inGame = reader.ReadBoolean();
			var lookingForGame = reader.ReadBoolean();
			var turnOrder = new TurnOrder
			{
				TurnIndex = reader.ReadByte(),
				ActionIndex = reader.ReadByte()
			};

			player.GetModPlayer<GameStateSyncPlayer>().NetworkUpdate(inGame, lookingForGame, turnOrder);

			if(Main.netMode == NetmodeID.Server) 
			{
				new PlayerGameStatePacket(player, inGame, lookingForGame, turnOrder).Send(to: -1, from: sender);
			}
		}

		protected override void PostSend(BinaryWriter writer, Player player)
		{
			writer.Write(inGame);
			writer.Write(lookingForGame);
			writer.Write((byte)turnOrder.TurnIndex);
			writer.Write((byte)turnOrder.ActionIndex);
		}
	}
}
