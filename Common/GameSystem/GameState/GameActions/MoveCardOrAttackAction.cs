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
                startZone.Animation = new RemoveCardAnimation(startZone, endZone.PlacedCard, Main._drawInterfaceGameTime.TotalGameTime);
                endZone.Animation = new PlaceCardAnimation(endZone, Main._drawInterfaceGameTime.TotalGameTime);
            } else
            {
                var currTime = Main._drawInterfaceGameTime.TotalGameTime;
                // attack opposing field
                var prevHealth = endZone.PlacedCard.CurrentHealth;
                endZone.PlacedCard.CurrentHealth -= startZone.PlacedCard.Template.Attacks[0].Damage;
                startZone.Animation = new MeleeAttackAnimation(startZone, endZone, currTime);
                if(endZone.PlacedCard.CurrentHealth > 0)
                {
                    endZone.Animation = new TakeDamageAnimation(endZone, currTime, TimeSpan.FromSeconds(0.5f), prevHealth, endZone.Animation.StartTime);
                } else
                {
                    endZone.Animation = new DeathAnimation(
                        endZone, currTime, TimeSpan.FromSeconds(0.5f), prevHealth, endZone.PlacedCard, endZone.Animation.StartTime);
                    endZone.PlacedCard = null;
                }
            }
        }

        public void Cancel()
        {
            // No-op
        }
    }
}
