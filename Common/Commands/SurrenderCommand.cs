using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.Commands
{
    internal class SurrenderCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "surrender";

        public override string Description => "Surrender an active TerraTCG game";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if(caller.Player.whoAmI == Main.myPlayer)
            {
                TCGPlayer.LocalGamePlayer.Resources =
                    TCGPlayer.LocalGamePlayer.Resources.UseResource(health: TCGPlayer.LocalGamePlayer.Resources.Health);
            }
        }
    }
}
