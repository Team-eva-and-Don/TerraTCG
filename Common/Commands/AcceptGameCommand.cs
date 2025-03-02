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
    internal class AcceptGameCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "accept";

        public override string Description => "Start a networked TerraTCG game!";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
			var matchmaker = MatchmakingSystem.Instance;
            if(caller.Player.whoAmI == Main.myPlayer && matchmaker.NextPlayer is Player opponent)
            {
				matchmaker.RemoveLookingForGamePlayer(opponent);
                var myPlayer = TCGPlayer.LocalPlayer;
				var opponentController = new NoOpNetGamePlayerController(); // Replaced with real opponent during deck sync

                ModContent.GetInstance<GameModSystem>().StartGame(myPlayer, opponentController, 0);
				// Send out a net packet to trigger a game with another player
				var handAndDeck = new CardCollection()
				{
					Cards = [.. myPlayer.GamePlayer.Deck.Cards, .. myPlayer.GamePlayer.Hand.Cards]
				};
				GameActionPacketQueue.Instance.QueueOutgoingMessage(
					new DecklistPacket(myPlayer.Player, opponent.whoAmI, handAndDeck));
            }
        }
    }
}
