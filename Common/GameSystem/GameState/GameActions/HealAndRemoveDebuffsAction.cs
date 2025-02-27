using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations;
using TerraTCG.Common.GameSystem.GameState.Modifiers;
using static TerraTCG.Common.GameSystem.GameState.GameActions.IGameAction;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    internal class HealAndRemoveDebuffsAction : TownsfolkAction
    {
        private Zone zone;

		public HealAndRemoveDebuffsAction() : base() { }

		public HealAndRemoveDebuffsAction(Card card, GamePlayer player) : base(card, player) { }

        public override ActionLogInfo GetLogMessage() => new(Card, $"{ActionText("Used")} {Card.CardName} {ActionText("On")} {zone.CardName}");

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

			zone.PlacedCard.Heal(3);
			zone.PlacedCard.CardModifiers = zone.PlacedCard.CardModifiers
				.Where(m => m.Category != ModifierType.POISON && m.Category != ModifierType.BLEEDING)
				.ToList();

            zone.QueueAnimation(new IdleAnimation(zone.PlacedCard, duration));
            zone.QueueAnimation(new ActionAnimation(zone.PlacedCard));
            GameSounds.PlaySound(GameAction.USE_SKILL);
        }
    }
}
