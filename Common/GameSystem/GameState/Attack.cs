using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal struct Attack
    {
        internal string Name { get; set; }
        internal string Description { get; set; }
        internal int Damage { get; set; }
        internal int Cost { get; set; }
    }
}
