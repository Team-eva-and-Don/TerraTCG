using System;
using System.Collections.Generic;
using System.IO;
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
    internal class ReduceAttackCostAction : TownsfolkAction
    {
        private Zone zone;

		public ReduceAttackCostAction() : base() { }

		public ReduceAttackCostAction(Card card, GamePlayer player) : base(card, player) { }

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

            zone.PlacedCard.IsExerted = false;
            zone.PlacedCard.AddModifiers([new AttackCostReductionModifier(1, removeOn: [GameEvent.END_TURN])]);

            zone.QueueAnimation(new IdleAnimation(zone.PlacedCard, duration));
            zone.QueueAnimation(new ActionAnimation(zone.PlacedCard));
            GameSounds.PlaySound(GameAction.USE_SKILL);
        }

		public override void PostSend(BinaryWriter writer)
		{
			writer.Write((byte)zone.Index);
		}

		public override void PostReceive(BinaryReader reader, CardGame game)
		{
			zone = Player.Field.Zones[reader.ReadByte()];
		}
    }
}
