using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal class PlacedCard(Card template)
    {
        internal Card Template { get; set; } = template;

        internal int CurrentHealth { get; set; } = template.MaxHealth;
    }
}
