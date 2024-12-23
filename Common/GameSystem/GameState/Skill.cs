using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraTCG.Common.GameSystem.GameState.GameActions;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal delegate void DoSkill(GamePlayer player, Zone cardZone, Zone endZone);
    internal struct Skill()
    {

        internal string Name { get; set; }
        internal int Cost { get; set; }
        internal string Description { get; set; }

        // Flag used by bots to determine how to select targets for skills
        internal ZoneRole Role { get; set; } = ZoneRole.OFFENSE;

        internal ActionType SkillType { get; set; } = ActionType.SKILL;
        internal DoSkill DoSkill { get; set; }

    }
}
