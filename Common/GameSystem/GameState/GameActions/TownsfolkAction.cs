using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    internal abstract class TownsfolkAction(Card card, GamePlayer player) : IGameAction
    {
        internal Card Card { get; } = card;
        internal GamePlayer Player { get; } = player;
        public virtual bool CanAcceptZone(Zone zone) => Player.Resources.TownsfolkMana > 0;
        public abstract bool AcceptZone(Zone zone);

        public virtual void Complete()
        {
            Player.Resources = Player.Resources.UseResource(townsfolkMana: 1);
            Player.Hand.Remove(Card);
        }
    }
}
