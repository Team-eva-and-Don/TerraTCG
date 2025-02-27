using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations;
using static TerraTCG.Common.GameSystem.GameState.GameActions.IGameAction;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    internal class BounceCardAction : TownsfolkAction
    {
        private Zone zone;

		public BounceCardAction() : base() { }

		public BounceCardAction(Card card, GamePlayer player) : base(card, player) { }

        public override ActionLogInfo GetLogMessage() => new(Card, $"{ActionText("Bounced")} {ActionText("With")} {Card.CardName}");

        public override bool CanAcceptZone(Zone zone) => base.CanAcceptZone(zone) && 
			Player.Owns(zone) && !zone.IsEmpty() && !zone.PlacedCard.Template.SubTypes.Contains(CardSubtype.EXPERT) &&
			// Don't allow returning "tokens" to the hand
			zone.PlacedCard.Template.IsCollectable &&
			// Don't allow returning cards that attacked this turn to the hand
			!zone.PlacedCard.IsExerted;

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
            GameSounds.PlaySound(GameAction.BOUNCE_CARD);
        }
    }
}
