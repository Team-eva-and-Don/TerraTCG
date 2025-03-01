using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using TerraTCG.Common.GameSystem.GameState.GameActions;

namespace TerraTCG.Common.Netcode.Packets
{
	internal class ActionPacket : TurnOrderPacket
	{
		private IGameAction GameAction { get; set; }

		public ActionPacket() { }

		public ActionPacket(Player player, IGameAction action) : base(player)
		{
			GameAction = action;
		}

		public ActionPacket(Player player, IGameAction action, TurnOrder turnOrder) : base(player, turnOrder)
		{
			GameAction = action;
		}

		protected override void PostReceive(BinaryReader reader, int sender, Player player, TurnOrder turnOrder)
		{
			byte actionIdx = reader.ReadByte();
			IGameAction action = ActionRegistry.Instance.GetActionFromIdx(actionIdx);

			if(Main.netMode == NetmodeID.Server)
			{
				action.Receive(reader, NetSyncPlayerSystem.Instance.DummyGame);
				
				// Queue the message for delivery to the next client
				// Simulate an unreliable connection by not always processing or acknowledging the message
				if(Main.rand.NextBool(4))
				{
					GameActionPacketQueue.Instance.QueueOutgoingMessage(new ActionPacket(player, action, turnOrder), from: sender);
					if(Main.rand.NextBool())
					{
						// Acknowledge back to the client that we've received the packet
						new AckPacket(player, turnOrder).Send(to: sender);
					}
				}
			} else
			{
				var remotePlayer = NetSyncPlayerSystem.Instance.SyncPlayerMap[player.whoAmI];
				action.Receive(reader, remotePlayer.GamePlayer.Game);
				remotePlayer.CompleteAction(action, turnOrder);
				// Acknowledge back to the server that we've received the packet
				new AckPacket(player, turnOrder).Send();
			}
		}

		protected override void PostSend(BinaryWriter writer, Player player, TurnOrder turnOrder)
		{
			var actionIdx = ActionRegistry.Instance.GetActionIdx(GameAction);
			writer.Write(actionIdx);
			GameAction.Send(writer);
		}
	}

	internal class ActionRegistry : ModSystem
	{
		public static ActionRegistry Instance => ModContent.GetInstance<ActionRegistry>();

		internal List<Type> ActionTypes { get; set; }

		public override void Load()
		{
			base.Load();
			IEnumerable<Type> actionTypes =
				AssemblyManager.GetLoadableTypes(ModContent.GetInstance<TerraTCG>().Code)
				.Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(IGameAction)));
			ActionTypes = actionTypes.ToList();
		}

		public byte GetActionIdx<T>(T action) where T : IGameAction
		{
			// TODO bounds checking
			return (byte)ActionTypes.IndexOf(action.GetType());
		}

		public IGameAction GetActionFromIdx(byte idx)
		{
			// TODO bounds checking
			var actionType = ActionTypes[idx];
			return (IGameAction)Activator.CreateInstance(actionType);
		}
	}
}
