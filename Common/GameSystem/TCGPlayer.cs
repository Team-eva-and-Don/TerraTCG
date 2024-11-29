using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem
{
    internal class TCGPlayer : ModPlayer
    {

        internal GamePlayer GamePlayer { get; set; }

        public override void OnEnterWorld()
        {
            ModContent.GetInstance<FieldRenderer>().OnEnterWorld();
            ModContent.GetInstance<GameModSystem>().StartGame(this);
        }
    }
}
