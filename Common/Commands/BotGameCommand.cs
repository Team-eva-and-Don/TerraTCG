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
    internal class BotGameCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "botgame";

        public override string Description => "Start a TerraTCG game against a bot opponent";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            ModContent.GetInstance<GameModSystem>().StartGame(
                Main.LocalPlayer.GetModPlayer<TCGPlayer>(),
                new SimpleBotPlayer());
        }
    }
}
