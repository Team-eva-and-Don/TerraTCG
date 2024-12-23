using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    internal class UnpauseCardAction(Card card, GamePlayer player) : TownsfolkAction(card, player)
    {
        private Zone zone;

        public override bool CanAcceptZone(Zone zone) => base.CanAcceptZone(zone) 
            && Player.Owns(zone) && !zone.IsEmpty() && zone.PlacedCard.IsExerted;

        public override bool AcceptZone(Zone zone)
        {
            this.zone = zone;
            return true;
        }

        public override Zone TargetZone() => zone;

        public override void Complete()
        {
            base.Complete();
            var duration = GetAnimationStartDelay();

            zone.PlacedCard.IsExerted = false;

            zone.QueueAnimation(new IdleAnimation(zone.PlacedCard, duration, exertedOverride: true));
            zone.QueueAnimation(new BecomeActiveAnimation(zone.PlacedCard));
        }
    }
}
