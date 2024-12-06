using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing.Animations;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    internal class MoveCardAction(Card card, GamePlayer player, GamePlayer target) : IGameAction
    {
        private Zone sourceZone;
        private Zone destZone;

        private int Step => sourceZone == null ? 0 : 1;

        public bool CanAcceptZone(Zone zone) =>
            target.Owns(zone) && (Step == 0 ? !zone.IsEmpty() : zone.IsEmpty());

        public bool AcceptZone(Zone zone)
        {
            if(Step == 0)
            {
                sourceZone = zone;
            } else
            {
                destZone = zone;
            }
            return sourceZone != null && destZone != null;
        }

        public void Complete()
        {
            destZone.PlacedCard = sourceZone.PlacedCard;
            sourceZone.PlacedCard = null;
            sourceZone.Animation = new RemoveCardAnimation(sourceZone, destZone.PlacedCard, Main._drawInterfaceGameTime.TotalGameTime);
            destZone.Animation = new PlaceCardAnimation(destZone, Main._drawInterfaceGameTime.TotalGameTime);
            player.Hand.Remove(card);
        }
    }
}
