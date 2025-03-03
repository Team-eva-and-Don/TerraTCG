using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.Netcode;
using TerraTCG.Common.Netcode.Packets;

namespace TerraTCG.Common.Commands
{
    internal class LookingForGameCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "lfg";

        public override string Description => "Start a networked TerraTCG game!";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if(caller.Player.whoAmI == Main.myPlayer)
            {
				// Broadcast to other clients that you are looking for a game
				var syncPlayer = caller.Player.GetModPlayer<GameStateSyncPlayer>();
				syncPlayer.LookingForGame = true;
				syncPlayer.BroadcastSyncState();
            }
        }
    }
}
