using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerraTCG.Common.Netcode.Packets
{
	public abstract class MPPacket
	{
		//Shortcut
		public void Send(int to = -1, int from = -1, Func<Player, bool> bcCondition = null)
		{
			NetHandler.Send(this, to, from, bcCondition);
		}

		/// <summary>
		/// Write data to the packet
		/// </summary>
		public abstract void Send(BinaryWriter writer);

		/// <summary>
		/// Any class fields are possibly uninitialized here, they should only be used for Send
		/// </summary>
		public abstract void Receive(BinaryReader reader, int sender);
	}
}
