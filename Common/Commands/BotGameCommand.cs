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

        public override string Command => "bg";

        public override string Description => "Start a TerraTCG game against a bot opponent";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if(caller.Player.whoAmI == Main.myPlayer)
            {
                var myPlayer = TCGPlayer.LocalPlayer;
                var opponent = new SimpleBotPlayer();

                myPlayer.Deck = BotDecks.GetDeck(args.Length > 0 ? int.Parse(args[0]) : -1);
                opponent.Deck = BotDecks.GetDeck(args.Length > 1 ? int.Parse(args[1]) : -1);
                ModContent.GetInstance<GameModSystem>().StartGame(myPlayer, opponent);
            }
        }
    }
}
