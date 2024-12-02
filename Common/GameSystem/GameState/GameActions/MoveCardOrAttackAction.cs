using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing.Animations;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    // Action to either move a card to a neighboring zone or attack against an enemy creature
    // TODO we probably want to split those out, UI-permitting
    internal class MoveCardOrAttackAction(Zone startZone, GamePlayer player) : IGameAction
    {
        private Zone endZone;
        public bool CanAcceptCardInHand(Card card) => false;

        public bool CanAcceptZone(Zone zone) => (player.Owns(zone) && zone.IsEmpty()) || (!player.Owns(zone) && ! zone.IsEmpty());

        public bool AcceptCardInHand(Card card) => false;

        public bool AcceptZone(Zone zone)
        {
            endZone = zone;
            return true;
        }

        public void Complete()
        {
            if (player.Owns(endZone))
            {
                // move within own field
                endZone.PlacedCard = startZone.PlacedCard;
                startZone.PlacedCard = null;
                startZone.Animation = new RemoveCardAnimation(startZone, endZone.PlacedCard.Template, Main._drawInterfaceGameTime.TotalGameTime);
                endZone.Animation = new PlaceCardAnimation(endZone, Main._drawInterfaceGameTime.TotalGameTime);
            } else
            {
                // attack opposing field
                endZone.PlacedCard.CurrentHealth -= startZone.PlacedCard.Template.Attacks[0].Damage;
            }
        }

        public void Cancel()
        {
            // No-op
        }
    }
}
