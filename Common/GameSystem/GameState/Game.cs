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
    internal class Game
    {
        internal List<GamePlayer> GamePlayers { get; set; }

        internal List<Turn> Turns { get; set; } = [];

        internal IFieldAnimation FieldAnimation { get; set; }

        internal Turn CurrentTurn
        {
            get => Turns.Last();
            set => Turns.Add(value);
        }

        public void StartGame(TCGPlayer player, IBotPlayer botPlayer)
        {
            GamePlayers = [
                new GamePlayer(this),
                new GamePlayer(this)
            ];
            player.GamePlayer = GamePlayers[0];

            // Put a reference set of enemies onto the opponent's board
            GamePlayers[1].Field.Zones[0].PlaceCard(Skeleton.Instance.CreateCard());
            GamePlayers[1].Field.Zones[2].PlaceCard(Bat.Instance.CreateCard());

            for(int i =  0; i < GamePlayers[1].Field.Zones.Count; i++)
            {
                var zone = GamePlayers[1].Field.Zones[i];
                if(!zone.IsEmpty())
                {
                    zone.Animation = new IdleAnimation(zone, TimeSpan.FromSeconds(-i/3f));
                }
            }

            BotPlayerSystem.Instance.RegisterBotPlayer(botPlayer, this, GamePlayers[1]);

            CurrentTurn = new()
            {
                Game = this,
                ActivePlayer = GamePlayers[0],
                TurnCount = 1
            };
            CurrentTurn.Start();
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
    }

    internal class GameModSystem : ModSystem
    {
        internal List<Game> ActiveGames { get; set; }

        public override void Load()
        {
            ActiveGames = [];
        }


        public Game StartGame(TCGPlayer player)
        {
            var game = new Game();
            game.StartGame(player, new SimpleBotPlayer());
            ActiveGames.Add(game);
            return game;
        }
    }
}
