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
    internal class RampAction(Card card, GamePlayer player, int amount=1) : TownsfolkAction(card, player), IGameAction
    {
        public override bool AcceptZone(Zone zone) => false;

        public override Zone TargetZone() => null;

        public bool CanAcceptActionButton() => Player.Resources.TownsfolkMana > 0;

        public bool AcceptActionButton() => true;

        public override void Complete()
        {
            base.Complete();
            Player.ManaPerTurn += amount;
        }
    }
}
