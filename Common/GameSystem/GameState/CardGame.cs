using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal class CardGame
    {
        internal List<GamePlayer> GamePlayers { get; set; }

        internal List<IGamePlayerController> GamePlayerControllers { get; set; }

        internal List<Turn> Turns { get; set; } = [];

        internal IFieldAnimation FieldAnimation { get; set; }

        internal Turn CurrentTurn
        {
            get => Turns.Last();
            set => Turns.Add(value);
        }

        internal bool IsActive { get; private set; } = true;

        public void StartGame(IGamePlayerController player1, IGamePlayerController player2)
        {
            GamePlayers = [
                new GamePlayer(this),
                new GamePlayer(this)
            ];

            GamePlayerControllers = [
                player1, 
                player2
            ];

            for(int i =  0; i < GamePlayers[1].Field.Zones.Count; i++)
            {
                var zone = GamePlayers[1].Field.Zones[i];
                if(!zone.IsEmpty())
                {
                    zone.Animation = new IdleAnimation(zone, TimeSpan.FromSeconds(-i/3f));
                }
            }

            for(int i = 0; i < GamePlayers.Count; i++)
            {
                GamePlayerControllers[i].StartGame(GamePlayers[i], this);
            }

            CurrentTurn = new()
            {
                Game = this,
                ActivePlayer = GamePlayers[0],
                TurnCount = 1
            };
            CurrentTurn.Start();
        }
        public void EndGame()
        {
            // de-register game players
            foreach(var controller in GamePlayerControllers)
            {
                controller.EndGame();
            }
            IsActive = false;
        }

        public IEnumerable<Zone> AllZones() =>
            GamePlayers[0].Field.Zones.Concat(GamePlayers[1].Field.Zones);

        // Utility method used by bots to check whether any action animation is still in progress
        internal bool IsDoingAnimation()
        {
            if(FieldAnimation != null)
            {
                return true;
            }
            foreach (var zone in AllZones())
            {
                if(!(zone.Animation?.IsDefault() ?? true))
                {
                    return true;
                }
            }
            return false;
        }

        // Check for state based actions, such as the game ending when a player has zero hp
        public void CheckStateActions()
        {
            if(IsDoingAnimation())
            {
                // allow animations to finish before updating game state
                return; 
            }
            foreach(var player in GamePlayers)
            {
                if(player.Resources.Health <= 0)
                {
                    EndGame();
                    break;
                }
            }
        }
    }

    internal class GameModSystem : ModSystem
    {
        internal List<CardGame> ActiveGames { get; set; }

        public override void Load()
        {
            ActiveGames = [];
        }


        public CardGame StartGame(IGamePlayerController player1, IGamePlayerController player2)
        {
            var game = new CardGame();
            game.StartGame(player1, player2);
            ActiveGames.Add(game);
            return game;
        }

        public override void PreUpdatePlayers()
        {
            foreach (var game in ActiveGames)
            {
                game.CheckStateActions();
            }
            // games end via state check, prune them
            ActiveGames = ActiveGames.Where(g => g.IsActive).ToList();
        }

    }
}
