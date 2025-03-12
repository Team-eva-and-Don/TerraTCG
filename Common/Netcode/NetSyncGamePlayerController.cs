using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.Netcode.Packets;
using TerraTCG.Content.NPCs;

namespace TerraTCG.Common.Netcode
{
	// Implementation of IGamePlayerController that performs actions based on 
	internal class NetSyncGamePlayerController : IBotPlayer
	{

		public int PlayerId { get; set; }
		public string Name { get; set; }

		public GamePlayer GamePlayer { get ; set ; }
		public CardCollection Deck { get ; set ; }
		public string DeckName { get ; set ; }


		public bool ShouldShuffle { get => false; }

		public List<NPCDuelReward> Rewards { get; set; }

		public Asset<Texture2D> Sleeve => TextureCache.Instance.CardSleeves[CardSleeve.FOREST];

		// Keep track of the actions already served this game to prevent duplication
		private List<TurnOrder> CompletedActions { get; set; } = [];

		public bool NotYetPerformed(TurnOrder sequence)
		{
			var notYetPerformed = !CompletedActions.Contains(sequence);
			if(notYetPerformed)
			{
				CompletedActions.Add(sequence);
			}
			return notYetPerformed;
		}
		public void EndGame()
		{
			NetSyncPlayerSystem.Instance.UnregisterPlayer(PlayerId);
			ModContent.GetInstance<BotPlayerSystem>().UnregisterBotPlayer(this);
		}

		public void StartGame(GamePlayer player, CardGame game)
		{
			GamePlayer = player;
		}

		public void CompleteAction(IGameAction action, TurnOrder sequence)
		{
			if (NotYetPerformed(sequence))
			{
				GamePlayer?.Game.LogAndCompleteAction(action);
			}
		}

		internal void PassTurn(TurnOrder sequence)
		{
			if (NotYetPerformed(sequence))
			{
				GamePlayer?.PassTurn();
			}
		}

		internal void Surrender(TurnOrder sequence)
		{
			if(NotYetPerformed(sequence))
			{
				GamePlayer.Surrender();
			}
		}

		public void Update()
		{
			// No-op
		}
	}

	// Dummy GamePlayer used as placeholder when starting network-initiated games
	// Auto-concedes after 5 seconds to bail out of scenarios where handshake
	// fails to complete
	internal class NoOpNetGamePlayerController : IBotPlayer
	{
		public GamePlayer GamePlayer { get ; set ; }
		public CardCollection Deck { get; set; } = BotDecks.GetStarterDeck();
		public string DeckName { get ; set ; }

		public bool ShouldShuffle { get => false; }

		public List<NPCDuelReward> Rewards { get; set; }

		public Asset<Texture2D> Sleeve => TextureCache.Instance.CardSleeves[CardSleeve.FOREST];

		public void StartGame(GamePlayer player, CardGame game)
		{
			GamePlayer = player;
		}

        public void EndGame()
        {
            // de-register myself
            ModContent.GetInstance<BotPlayerSystem>().UnregisterBotPlayer(this);
        }

		public void Update()
		{
			if(TCGPlayer.TotalGameTime - GamePlayer.Game.StartTime > TimeSpan.FromSeconds(5f))
			{
				GamePlayer.Surrender();
				Main.NewText("ERROR: Failed to complete start-of-game network exchange. Please contact the developer.", Color.Red);
			}
		}
	}

	internal class NetSyncPlayerSystem : ModSystem
	{
		public static NetSyncPlayerSystem Instance => ModContent.GetInstance<NetSyncPlayerSystem>();

		private CardGame _dummyGame;

		// Singleton instance of a card game used to de/re-serialize game actions server side

		public List<IGamePlayerController> DummyControllers { get; } = [new NoOpNetGamePlayerController(), new NoOpNetGamePlayerController()];
		public CardGame DummyGame { 
			get
			{
				if(_dummyGame == null)
				{
					_dummyGame = new CardGame();
					_dummyGame.StartGame(DummyControllers[0], DummyControllers[1]);
				}
				return _dummyGame;
			} 
		}

		public Dictionary<int, NetSyncGamePlayerController> SyncPlayerMap { get; private set; } = [];

		public NetSyncGamePlayerController RegisterPlayer(int playerId, CardCollection playerDecklist)
		{
			var controller = new NetSyncGamePlayerController()
			{
				PlayerId = playerId,
				Name = Main.player[playerId].name,
				Deck = playerDecklist,
			};

			SyncPlayerMap[playerId] = controller;
			BotPlayerSystem.Instance.RegisterBotPlayer(controller);
			return controller;
		}

		public void UnregisterPlayer(int playerId)
		{
			SyncPlayerMap.Remove(playerId);
		}
	}
}
