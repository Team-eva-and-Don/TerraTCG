using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TerraTCG.Common.Netcode
{
	internal class MatchmakingSystem : ModSystem
	{
		public static MatchmakingSystem Instance => ModContent.GetInstance<MatchmakingSystem>();

		public List<Player> PendingPlayerMatches { get; set; } = [];

		public Player NextPlayer => PendingPlayerMatches.FirstOrDefault();

		// Remove a player from the queue of looking for game players
		public void AddLookingForGamePlayer(Player player)
		{
			if (!PendingPlayerMatches.Contains(player))
			{
				PendingPlayerMatches.Add(player);
			}
		}

		// Add a player to the queue of looking for game players
		public void RemoveLookingForGamePlayer(Player player)
		{
			PendingPlayerMatches.Remove(player);
		}
	}
}
