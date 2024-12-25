using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    internal class ReduceAttackCostAction(Card card, GamePlayer player) : TownsfolkAction(card, player)
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

            zone.PlacedCard.IsExerted = false;
            zone.PlacedCard.AddModifiers([new AttackCostReductionModifier(1, removeOn: [GameEvent.END_TURN])]);

            zone.QueueAnimation(new IdleAnimation(zone.PlacedCard, duration));
            zone.QueueAnimation(new ActionAnimation(zone.PlacedCard));
            base.Complete();
        }
    }
}
