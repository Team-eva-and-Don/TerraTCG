using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    internal delegate IGameAction SelectInHandAction(Card card, GamePlayer gamePlayer);

    internal delegate IGameAction SelectOnFieldAction(Zone zone, GamePlayer gamePlayer);

    // Interface for a state machine that accepts player input until
    // enough info has been gathered to perform an action
    internal interface IGameAction
    {
        public bool CanAcceptZone(Zone zone);
        public bool AcceptZone(Zone zone);

        public bool CanAcceptCardInHand(Card card);

        public bool AcceptCardInHand(Card card);

        public void Complete();

        public void Cancel();
    }
}
