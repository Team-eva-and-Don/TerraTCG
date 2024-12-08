using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal delegate void DoSkill(GamePlayer player, Zone cardZone);
    internal struct Skill
    {

        internal string Name { get; set; }
        internal int Cost { get; set; }
        internal string Description { get; set; }

        internal DoSkill DoSkill { get; set; }

    }
}
