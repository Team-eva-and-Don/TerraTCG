using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI;

namespace TerraTCG.Common.GameSystem
{
    internal interface IGamePlayerController
    {
        public GamePlayer GamePlayer {get; set;}

        public void StartGame(GamePlayer player, CardGame game);

        public void EndGame();
    }

    internal class TCGPlayer : ModPlayer, IGamePlayerController
    {

        internal static TCGPlayer LocalPlayer => Main.LocalPlayer.GetModPlayer<TCGPlayer>();
        internal static GamePlayer LocalGamePlayer => LocalPlayer.GamePlayer;
        internal static TimeSpan TotalGameTime => Main._drawInterfaceGameTime?.TotalGameTime ?? TimeSpan.FromSeconds(0);

        public GamePlayer GamePlayer { get; set; }

        // TODO this is not the correct place to cache this info, but is the easiest
        // Place within UI coordinates that the bottom center of the player's
        // back-center game zone is drawn
        internal Vector2 GameFieldPosition { get; set; }

        public override void OnEnterWorld()
        {
            if(Player.whoAmI == Main.myPlayer)
            {
                ModContent.GetInstance<FieldRenderer>().OnEnterWorld();
            }
        }

        public void StartGame(GamePlayer player, CardGame game)
        {
            GamePlayer = player;
            ModContent.GetInstance<UserInterfaces>().StartGame();
        }

        public void EndGame()
        {
            GamePlayer = null;
            ModContent.GetInstance<UserInterfaces>().EndGame();
        }
    }
}
