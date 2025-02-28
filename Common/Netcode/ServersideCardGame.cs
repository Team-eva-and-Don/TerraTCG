using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.Netcode
{
	internal class ServersideCardGame : CardGame
	{
        public override void StartGame(IGamePlayerController player1, IGamePlayerController player2, int? startIdx = null)
        {
            GamePlayers = [
                new GamePlayer(this, player1.Deck.Copy(), player1),
                new GamePlayer(this, player2.Deck.Copy(), player2)
            ];

            GamePlayerControllers = [
                player1, 
                player2
            ];

            for(int i = 0; i < GamePlayers.Count; i++)
            {
                GamePlayerControllers[i].StartGame(GamePlayers[i], this);
            }

            CurrentTurn = new()
            {
                Game = this,
                ActivePlayer = GamePlayers[startIdx ?? Main.rand.Next(2)],
                TurnCount = 1
            };
            CurrentTurn.ActivePlayer.Opponent.ManaPerTurn += 1;
            CurrentTurn.Start();
        }
	}
}
