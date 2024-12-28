using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal interface ICardTemplate
    {
        public Card Card { get; }
        public Card CreateCard();
    }


    internal abstract class BaseCardTemplate : ModSystem, ICardTemplate
    {
        private Card _card;
        public Card Card { 
            get
            {
                _card ??= CreateCard();
                return _card;
            } 
        }

        public abstract Card CreateCard();
    }
}
