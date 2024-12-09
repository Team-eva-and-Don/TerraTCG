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

namespace TerraTCG.Common.GameSystem.GameState
{
    internal class Game
    {
        internal List<GamePlayer> GamePlayers { get; set; }

        internal List<Turn> Turns { get; set; } = [];

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
            GamePlayers[1].Field.Zones[1].PlaceCard(Zombie.Instance.CreateCard());
            GamePlayers[1].Field.Zones[2].PlaceCard(DemonEye.Instance.CreateCard());
            GamePlayers[1].Field.Zones[4].PlaceCard(Bunny.Instance.CreateCard());

            foreach(var zone in GamePlayers[1].Field.Zones)
            {
                if(!zone.IsEmpty())
                {
                    zone.Animation = new IdleAnimation(zone, TimeSpan.FromSeconds(0));
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
