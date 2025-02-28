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
    internal class BotGameCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "bg";

        public override string Description => "Start a networked TerraTCG game against a bot opponent!";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if(caller.Player.whoAmI == Main.myPlayer)
            {
                var myPlayer = TCGPlayer.LocalPlayer;
				var opponent = new NoOpNetGamePlayerController();

                var game = ModContent.GetInstance<GameModSystem>().StartGame(myPlayer, opponent, 0);
				// Send out a net packet to trigger a game with another player
				if(Main.netMode == NetmodeID.MultiplayerClient)
				{
					var handAndDeck = new CardCollection()
					{
						Cards = [.. myPlayer.GamePlayer.Deck.Cards, .. myPlayer.GamePlayer.Hand.Cards]
					};
					new DecklistPacket(myPlayer.Player, handAndDeck).Send(-1);
				}

            }
        }
    }
}
