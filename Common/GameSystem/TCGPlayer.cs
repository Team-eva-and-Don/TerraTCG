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

namespace TerraTCG.Common.GameSystem
{
    internal class TCGPlayer : ModPlayer
    {

        internal static GamePlayer LocalGamePlayer => Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;

        internal GamePlayer GamePlayer { get; set; }

        // TODO this is not the correct place to cache this info, but is the easiest
        // Place within UI coordinates that the bottom center of the player's
        // back-center game zone is drawn
        internal Vector2 GameFieldPosition { get; set; }

        public override void OnEnterWorld()
        {
            ModContent.GetInstance<FieldRenderer>().OnEnterWorld();
            ModContent.GetInstance<GameModSystem>().StartGame(this);
        }
    }
}
