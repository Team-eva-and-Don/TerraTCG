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
using TerraTCG.Common.UI;

namespace TerraTCG.Common.Commands
{
    internal class DeckbuildCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "db";

        public override string Description => "Open the TerraTCG Deckbuilder";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if(caller.Player.whoAmI == Main.myPlayer)
            {
                ModContent.GetInstance<UserInterfaces>().StartDeckbuild();
            }
        }
    }
}
