using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraTCG.Common.GameSystem.CardData;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal class GamePlayer
    {
        internal Game Game { get; set; } // back reference to the game this player belongs to
        internal CardCollection Hand { get; set; }
        internal CardCollection Deck { get; set; }

        internal Field Field { get; set; }

        internal Card SelectedHandCard { get; set; }

        internal Zone SelectedFieldZone { get; set; }
        public GamePlayer Opponent => Game.GamePlayers.Find(p => p != this);

        public GamePlayer()
        {
            Hand = new CardCollection()
            {
                Cards = [Zombie.Instance.CreateCard(), Bunny.Instance.CreateCard(), DemonEye.Instance.CreateCard()]
            };

            Deck = new CardCollection()
            {
                Cards = [Zombie.Instance.CreateCard(), Zombie.Instance.CreateCard(), Zombie.Instance.CreateCard()]
            };

            Field = new();

            SelectedHandCard = Hand.Cards[1];

            SelectedFieldZone = Field.Zones[0];
        }
    }
}
