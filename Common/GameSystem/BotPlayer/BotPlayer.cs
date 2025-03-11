using Microsoft.Xna.Framework;
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
    internal interface IBotPlayer : IGamePlayerController
    {
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

        // Games happen at the UI layer, run bot updates on each UI tick
        public override void UpdateUI(GameTime gameTime)
        {
            foreach(var player in Players)
            {
                player.Update();
            }
        }

        public void RegisterBotPlayer(IBotPlayer botPlayer)
        {
            Players.Add(botPlayer);
        }

        internal void UnregisterBotPlayer(IBotPlayer botPlayer)
        {
            Players.Remove(botPlayer);
        }
    }
}
