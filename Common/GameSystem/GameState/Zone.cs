using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal enum ZoneRole
    {
        OFFENSE,
        DEFENSE
    }
    internal class Zone
    {
        internal PlacedCard PlacedCard { get; set; }
        internal ZoneRole Role { get; set; }

        public void PlaceCard(Card card)
        {
            PlacedCard = new PlacedCard(card);
        }
    }
}
