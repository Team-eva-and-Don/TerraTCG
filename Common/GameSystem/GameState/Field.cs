using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal class Field
    {
        internal List<Zone> Zones { get; set; }

        public Field()
        {
            Zones = [
                new() { Role = ZoneRole.OFFENSE},
                new() { Role = ZoneRole.OFFENSE},
                new() { Role = ZoneRole.OFFENSE},
                new() { Role = ZoneRole.DEFENSE},
                new() { Role = ZoneRole.DEFENSE},
                new() { Role = ZoneRole.DEFENSE},
            ];
        }

    }
}
