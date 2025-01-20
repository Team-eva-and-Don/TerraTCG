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
    internal class SearchBossAction(Card card, GamePlayer player) : TownsfolkAction(card, player), IGameAction
    {
        public override ActionLogInfo GetLogMessage() => new(card, $"{ActionText("Used")} {Card.CardName}");

        public override bool CanAcceptZone(Zone zone) => false;

        public override bool AcceptZone(Zone zone) => false;

        public override Zone TargetZone() => null;

        public bool CanAcceptActionButton() => Player.Resources.TownsfolkMana > 0 && 
			Player.Resources.Health < Player.Opponent.Resources.Health &&
			Player.Deck.Cards.Any(c => c.SubTypes[0] == CardSubtype.BOSS);

        public bool AcceptActionButton() => true;

        public override void Complete()
        {
            base.Complete();

			var firstBoss = Player.Deck.Cards.Where(c => c.SubTypes[0] == CardSubtype.BOSS).FirstOrDefault();
			if(firstBoss != null)
			{
				Player.Deck.Cards.Remove(firstBoss);
				Player.Hand.Add(firstBoss);
			}
            GameSounds.PlaySound(GameAction.USE_SKILL);
        }
    }
}
