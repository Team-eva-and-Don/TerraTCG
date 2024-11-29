using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal class PlacedCard
    {
        internal Card Template { get; set; }

        internal int CurrentHealth { get; set; }

        public PlacedCard(Card template)
        {
            Template = template;
        }
    }
}
