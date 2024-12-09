using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.BotPlayer
{
    internal interface IBotPlayer
    {
        public void SetGame(Game game, GamePlayer player);
        public void Update();
    }
    internal class BotPlayerSystem : ModSystem
    {
        public static BotPlayerSystem Instance => ModContent.GetInstance<BotPlayerSystem>();

        private List<IBotPlayer> Players { get; set; }

        public override void Load()
        {
            Players = [];
        }

        public override void PreUpdatePlayers()
        {
            foreach(var player in Players)
            {
                player.Update();
            }
        }

        public void RegisterBotPlayer(IBotPlayer botPlayer, Game game, GamePlayer player)
        {
            botPlayer.SetGame(game, player);
            Players.Add(botPlayer);
        }

    }
}
