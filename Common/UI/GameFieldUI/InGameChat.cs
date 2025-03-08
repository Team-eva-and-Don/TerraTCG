using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem;

namespace TerraTCG.Common.UI.GameFieldUI
{
	// Set of hacks to invoke vanilla chat manager methods inside of the fancy UI
	// (which typically hides player chat)
	internal class InGameChat 
	{
		// TODO this is a fairly crude solution, we are not fully copying the 
		// vanilla state variables that control chat opening/closing though
		public static TimeSpan lastChatOpenTime;
		public static void TogglePlayerChat()
		{
			if(Main.keyState.IsKeyDown(Keys.Enter) && TCGPlayer.TotalGameTime - lastChatOpenTime > TimeSpan.FromSeconds(0.5f))
			{
				Main.OpenPlayerChat();
			} else if(Main.drawingPlayerChat)
			{
				lastChatOpenTime = TCGPlayer.TotalGameTime;
			} 		
		}

		public static void InvokeMainDrawPlayerChat()
		{
			// Only need to draw chat while in a multiplayer game
			if(!(TCGPlayer.LocalGamePlayer?.Game.IsMultiplayer ?? false))
			{
				return;
			}
			typeof(Main).GetMethod("DrawPlayerChat", BindingFlags.NonPublic | BindingFlags.Instance)
				.Invoke(Main.instance, []);
		}
	}
}
