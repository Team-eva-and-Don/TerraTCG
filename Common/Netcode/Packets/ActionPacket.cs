using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using TerraTCG.Common.GameSystem.GameState.GameActions;

namespace TerraTCG.Common.Netcode.Packets
{
	internal class ActionPacket : PlayerPacket
	{
		private IGameAction GameAction { get; set; }

		public ActionPacket() { }

		public ActionPacket(Player player, IGameAction action) : base(player)
		{
			GameAction = action;
		}

		protected override void PostReceive(BinaryReader reader, int sender, Player player)
		{
			throw new NotImplementedException();
		}

		protected override void PostSend(BinaryWriter writer, Player player)
		{
			byte actionType = (byte)ModContent.GetInstance<ActionRegistry>().ActionTypes.IndexOf(GameAction.GetType());
		}
	}

	internal class ActionRegistry : ModSystem
	{
		internal List<Type> ActionTypes { get; set; }

		public override void Load()
		{
			base.Load();
			IEnumerable<Type> actionTypes =
				AssemblyManager.GetLoadableTypes(ModContent.GetInstance<TerraTCG>().Code)
				.Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(IGameAction)));
			ActionTypes = actionTypes.ToList();
		}

		public byte GetActionIdx<T>(T _) where T : IGameAction
		{
			// TODO bounds checking
			return (byte)ActionTypes.IndexOf(typeof(T));
		}

		public IGameAction ConstructActionFromIdx(byte idx)
		{
			// TODO bounds checking
			var actionType = ActionTypes[idx];
			return (IGameAction)Activator.CreateInstance(actionType);
		}
	}
}
