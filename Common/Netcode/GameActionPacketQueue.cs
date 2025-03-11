using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.Netcode.Packets;

namespace TerraTCG.Common.Netcode
{
	internal class QueuedGameActionPacket
	{
		public int SendTo {get; set;}

		public int From {get; set;}

		public TurnOrder SortOrder { get; set; }

		public MPPacket Packet { get; set; }
		
		public TimeSpan LastSent { get; set; }
	}

	// Queue of not-yet-acknowledged outgoing network packets maintained on all clients
	// and the server. Clients queue actions for delivery to the server, server queues
	// actions from clients to forward to other clients
	internal class GameActionPacketQueue : ModSystem
	{
		public static GameActionPacketQueue Instance => ModContent.GetInstance<GameActionPacketQueue>();

		private List<QueuedGameActionPacket>[] ActionQueue { get; set; }

		public override void Load()
		{
			ActionQueue = new List<QueuedGameActionPacket>[Main.maxPlayers];
			for (int i = 0; i < ActionQueue.Length; i++)
			{
				ActionQueue[i] = [];
			}
		}

		// Add a new Multiplayer Packet to the outgoing queue for the given player
		public void QueueOutgoingMessage(TurnOrderPacket packet, int from = -1)
		{
			if(Main.netMode == NetmodeID.SinglePlayer)
			{
				// Short circuit out of this in single player
				return;
			}

			var playerQueue = ActionQueue[packet.WhoAmI];
			// Don't doubly queue a single message with the same sort order
			if (!playerQueue.Any(m => m.SortOrder.Equals(packet.TurnOrder)))
			{
				playerQueue.Add(new()
				{
					Packet = packet,
					SortOrder = packet.TurnOrder,
					SendTo = packet.OpponentId,
					From = from,
				});
			}
		}

		// Remove a packet from the outgoing queue once it's been acknowledged by its recipient
		public void DeuqueueOutgoingMessage(Player player, TurnOrder turnOrder)
		{
			if(Main.netMode == NetmodeID.SinglePlayer)
			{
				// Short circuit out of this in single player
				return;
			}
			ActionQueue[player.whoAmI] = ActionQueue[player.whoAmI].Where(a => !a.SortOrder.Equals(turnOrder)).ToList();
		}

		public override void PostUpdatePlayers() // TODO is this the right hook
		{
			if(Main.netMode == NetmodeID.SinglePlayer)
			{
				// Short circuit out of this in single player
				return;
			}
			var now = Main.netMode == NetmodeID.Server ? 
				TimeSpan.FromTicks(DateTime.Now.Ticks) : // TODO what time variables are naturally kept on the server?
				Main.gameTimeCache.TotalGameTime;
			foreach(var actionQueue in ActionQueue.Where(q=>q.Count > 0))
			{
				var nextMessage = actionQueue.First();
				if(now - nextMessage.LastSent > TimeSpan.FromSeconds(0.1f))
				{
					nextMessage.Packet.Send(to: nextMessage.SendTo, from: nextMessage.From);
					nextMessage.LastSent = now;
				}
			}
		}

		public void ClearPlayerQueue(Player player)
		{
			ActionQueue[player.whoAmI] = [];
		}
	}

	public class QueueClearModPlayer : ModPlayer
	{
	}
}
