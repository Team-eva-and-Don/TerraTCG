using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

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
            game.StartGame(player);
            ActiveGames.Add(game);
            return game;
        }
    }
}
