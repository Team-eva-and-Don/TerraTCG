using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations;
using TerraTCG.Common.GameSystem.GameState.GameActions;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal class CardGame
    {
        internal List<GamePlayer> GamePlayers { get; set; }

        internal List<IGamePlayerController> GamePlayerControllers { get; set; }

        internal List<Turn> Turns { get; set; } = [];

        internal IFieldAnimation FieldAnimation { get; set; }

        internal TimeSpan StartTime { get; private set; }
        internal TimeSpan EndTime { get; private set; }

        internal TimeSpan FadeOutTime { get; } = TimeSpan.FromSeconds(2f);

        private readonly Random random = new ();

        internal GamePlayer Winner { get; set; }
        internal Turn CurrentTurn
        {
            get => Turns.Last();
            set => Turns.Add(value);
        }

        internal bool IsActive => EndTime == default || TCGPlayer.TotalGameTime - EndTime < FadeOutTime;

        public void StartGame(IGamePlayerController player1, IGamePlayerController player2)
        {
            GamePlayers = [
                new GamePlayer(this, player1.Deck.Copy()),
                new GamePlayer(this, player2.Deck.Copy())
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
                ActivePlayer = GamePlayers[Math.Abs((int)random.NextInt64()) % 2],
                TurnCount = 1
            };
            CurrentTurn.ActivePlayer.Opponent.ManaPerTurn += 1;
            CurrentTurn.Start();

            StartTime = Main._drawInterfaceGameTime.TotalGameTime;
            SoundEngine.PlaySound(SoundID.MenuOpen);
        }

        // Start game wrap-up animations
        public void MarkGameComplete()
        {
            if(EndTime == default)
            {
                FieldAnimation =
                    new GameResultAnimation(TCGPlayer.TotalGameTime, Winner == TCGPlayer.LocalGamePlayer);
                EndTime = Main._drawInterfaceGameTime.TotalGameTime;
            }
        }

        // Wrap up game
        public void EndGame()
        {
            // de-register game players
            foreach(var controller in GamePlayerControllers)
            {
                controller.EndGame();
            }
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
                    Winner = player.Opponent;
                    MarkGameComplete();
                    break;
                }
            }
        }

        public void LogAndCompleteAction(IGameAction action)
        {
            action.Complete();
            var player = CurrentTurn.ActivePlayer == TCGPlayer.LocalGamePlayer ?
                "You " : "Opponent ";
            CurrentTurn.ActionLog.Add(player + action.GetLogMessage());
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

        // Games are played at the UI layer, check the game state at each UI tick.
        public override void UpdateUI(GameTime gameTime)
        {
            foreach (var game in ActiveGames)
            {
                game.CheckStateActions();
                if(!game.IsActive)
                {
                    game.EndGame();
                }
            }
            // games end via state check, prune them
            ActiveGames = ActiveGames.Where(g => g.IsActive).ToList();
        }

    }
}
