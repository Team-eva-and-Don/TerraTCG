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
    internal class BounceCardAction(Card card, GamePlayer player) : TownsfolkAction(card, player)
    {
        private Zone zone;

        public override bool CanAcceptZone(Zone zone) => base.CanAcceptZone(zone) && Player.Owns(zone) && !zone.IsEmpty();

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

            var leavingCard = zone.PlacedCard;
            zone.PlacedCard = null;

            zone.QueueAnimation(new IdleAnimation(leavingCard, duration));
            zone.QueueAnimation(new RemoveCardAnimation(leavingCard));

            Player.Hand.Add(leavingCard.Template);
        }
    }
}
