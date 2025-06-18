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
using TerraTCG.Common.GameSystem.GameState.Modifiers;
using TerraTCG.Common.Netcode;
using TerraTCG.Common.Netcode.Packets;
using static TerraTCG.Common.GameSystem.GameState.GameActions.IGameAction;

namespace TerraTCG.Common.GameSystem.GameState
{
	public class CardGame
    {
        internal List<GamePlayer> GamePlayers { get; set; }

        internal List<IGamePlayerController> GamePlayerControllers { get; set; }

        internal List<Turn> Turns { get; set; } = [];

        internal IFieldAnimation FieldAnimation { get; set; }

        internal TimeSpan StartTime { get; private set; }
		internal TimeSpan LastActionTime { get; set; }

        internal TimeSpan EndTime { get; private set; }

        internal TimeSpan FadeOutTime { get; } = TimeSpan.FromSeconds(2f);

        internal GamePlayer Winner { get; set; }
        internal Turn CurrentTurn
        {
            get => Turns.Last();
            set => Turns.Add(value);
        }

        internal bool IsActive => EndTime == default || TCGPlayer.TotalGameTime - EndTime < FadeOutTime;

		internal bool IsMultiplayer => GamePlayerControllers.Any(c => c is NetSyncGamePlayerController);

		internal bool IsNoOp => GamePlayerControllers.Any(c => c is NoOpNetGamePlayerController);


		public virtual void StartGame(IGamePlayerController player1, IGamePlayerController player2, int? startIdx = null)
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

			if(Main.netMode != NetmodeID.Server)
			{
				StartTime = Main._drawInterfaceGameTime.TotalGameTime;
				LastActionTime = StartTime;
				SoundEngine.PlaySound(SoundID.MenuOpen);
			}
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

		// Stop all ongoing animations, used to ensure game can clean up
		internal void ClearAnimations()
		{
			FieldAnimation = null;
			foreach(var zone in AllZones().Where(z=>!(z.Animation?.IsDefault() ?? true)))
			{
				zone.ClearAnimationQueue();
			}

		}

        // Check for state based actions, such as the game ending when a player has zero hp
        public void CheckStateActions()
        {
            if(IsDoingAnimation())
            {
                // allow animations to finish before updating game state
                return; 
            }
			// Check whether any creatures on board are currently dead
			foreach (var zone in AllZones())
			{
				if(zone.PlacedCard?.CurrentHealth <= 0)
				{
					var hasMorbid = zone.PlacedCard.CardModifiers.Any(m => m.Category == ModifierType.MORBID);
					var messageText = hasMorbid ?
						 $"{zone.PlacedCard.Template.CardName} {ActionText("Died")}. {ActionText("Morbid")}" :
						 $"{zone.PlacedCard.Template.CardName} {ActionText("Died")}";
					CurrentTurn.ActionLog.Add(new(zone.PlacedCard.Template, messageText));
					zone.QueueAnimation(new RemoveCardAnimation(zone.PlacedCard));
					zone.Owner.Resources = zone.Owner.Resources.UseResource(health: zone.PlacedCard.Template.Points);
					zone.Owner.Field.ClearModifiers(CurrentTurn.ActivePlayer, zone, GameEvent.CREATURE_DIED);
					zone.PlacedCard = null;
				}
			}
			// Check whether any players are AFK in the current multiplayer match
			if(IsMultiplayer && Main.netMode != NetmodeID.Server)
			{
				var elapsedTime = TCGPlayer.TotalGameTime - LastActionTime;
				// auto-surrender if the net-synced player is AFK for too long
				if (elapsedTime >= TimeSpan.FromSeconds(60) && elapsedTime < TimeSpan.FromSeconds(60.5f) && 
					FieldAnimation == null)
				{
					FieldAnimation = new AFKWarningAnimation(TCGPlayer.TotalGameTime, CurrentTurn);
				} else if (elapsedTime >= TimeSpan.FromSeconds(90))
				{
					CurrentTurn.ActivePlayer.Surrender();
				}

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
				ActionText("You") : CurrentTurn.ActivePlayer.Controller.Name;
            var info = action.GetLogMessage();
            var toLog = new ActionLogInfo(info.Card, player + " " + info.Message);
            CurrentTurn.ActionLog.Add(toLog);
			LastActionTime = TCGPlayer.TotalGameTime;

			if(IsMultiplayer && CurrentTurn.ActivePlayer == TCGPlayer.LocalGamePlayer)
			{
				GameActionPacketQueue.Instance.QueueOutgoingMessage(new ActionPacket(Main.LocalPlayer, action));
			}
        }

		// Swap in a new controller for the existing game,
		// Used to replace a placeholder dummy player with a real opponent in networked gameplay
		internal void SwapController(IGamePlayerController newController, CardCollection deckList, int replaceIdx = 1)
		{
			var gamePlayer = GamePlayers[replaceIdx];

			var oldController = gamePlayer.Controller;
			// Let the old controller clean itself up before swapping
			oldController.EndGame();

			GamePlayerControllers[replaceIdx] = newController;
			newController.GamePlayer = gamePlayer;
			gamePlayer.Controller = newController;
			gamePlayer.Deck = deckList;
			gamePlayer.Hand = new();
			for(int i = 0; i < 5; i++)
			{
				gamePlayer.Hand.Add(gamePlayer.Deck.Draw());
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


        public CardGame StartGame(IGamePlayerController player1, IGamePlayerController player2, int? startIdx = null)
        {
            var game = new CardGame();
            game.StartGame(player1, player2, startIdx);
            ActiveGames.Add(game);
            return game;
        }

		public void RemoveGame(CardGame game)
		{
			ActiveGames.Remove(game);
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
