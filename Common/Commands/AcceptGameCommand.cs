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

        public override string Command => "acc";

        public override string Description => "Start a networked TerraTCG game!";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
			if(caller.Player.whoAmI != Main.myPlayer)
			{
				return;
			}
			var playerId = int.Parse(args[0]);
			if (Main.player[playerId] is Player player && player.active && player.GetModPlayer<GameStateSyncPlayer>().LookingForGame)
			{
				var myPlayer = TCGPlayer.LocalPlayer;
				var opponentController = new NoOpNetGamePlayerController(); // Replaced with real opponent during deck sync
				// TODO allowing the client to decide who goes first is not great
				var goingFirst = Main.rand.Next() % 2;


				ModContent.GetInstance<GameModSystem>().StartGame(myPlayer, opponentController, goingFirst);
				// Send out a net packet to trigger a game with another player
				var handAndDeck = new CardCollection()
				{
					Cards = [.. myPlayer.GamePlayer.Deck.Cards, .. myPlayer.GamePlayer.Hand.Cards]
				};
				GameActionPacketQueue.Instance.QueueOutgoingMessage(
					new DecklistPacket(myPlayer.Player, playerId, goingFirst, handAndDeck));
			} else
			{
				Main.NewText($"Player with id {playerId} isn't looking for a game!");
			}
        }
    }
}
