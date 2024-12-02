using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.Drawing.Animations;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal class Game
    {
        internal List<GamePlayer> GamePlayers { get; set; }

        public void StartGame(TCGPlayer player)
        {
            GamePlayers = [
                new GamePlayer() { Game = this },
                new GamePlayer() { Game = this }
            ];
            player.GamePlayer = GamePlayers[0];

            // Put a reference enemy onto the opponent's board
            GamePlayers[1].Field.Zones[1].PlaceCard(Zombie.Instance.CreateCard());
            GamePlayers[1].Field.Zones[4].PlaceCard(Bunny.Instance.CreateCard());
            GamePlayers[1].Field.Zones[1].Animation = new IdleAnimation(
                GamePlayers[1].Field.Zones[1], Main._drawInterfaceGameTime.TotalGameTime);
            GamePlayers[1].Field.Zones[4].Animation = new IdleAnimation(
                GamePlayers[1].Field.Zones[4], Main._drawInterfaceGameTime.TotalGameTime - TimeSpan.FromSeconds(1f));
        }

        public IEnumerable<Zone> AllZones() =>
            GamePlayers[0].Field.Zones.Concat(GamePlayers[1].Field.Zones);
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
            game.StartGame(player);
            ActiveGames.Add(game);
            return game;
        }
    }
}
