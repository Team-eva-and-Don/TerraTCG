using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing;

namespace TerraTCG.Common.GameSystem
{
    internal class TCGPlayer : ModPlayer
    {

        public override void OnEnterWorld()
        {
            ModContent.GetInstance<FieldRenderer>().OnEnterWorld();
        }
    }
}
