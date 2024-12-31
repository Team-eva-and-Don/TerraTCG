using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    internal abstract class TownsfolkAction(Card card, GamePlayer player) : IGameAction
    {
        internal Card Card { get; } = card;
        internal GamePlayer Player { get; } = player;
        public virtual bool CanAcceptZone(Zone zone) => Player.Resources.TownsfolkMana > 0;
        public abstract bool AcceptZone(Zone zone);

        public abstract string GetLogMessage();

        public virtual Zone TargetZone() => null;

        public TimeSpan GetAnimationStartDelay() => ShowCardAnimation.DURATION;
            // Player == TCGPlayer.LocalGamePlayer ? TimeSpan.FromSeconds(0f) : ShowCardAnimation.DURATION;

        public virtual void Complete()
        {
            Player.Resources = Player.Resources.UseResource(townsfolkMana: 1);
            Player.Hand.Remove(Card);
            // if(Player != TCGPlayer.LocalGamePlayer)
            {
                Player.Game.FieldAnimation = new ShowCardAnimation(TCGPlayer.TotalGameTime, Card, TargetZone());
            }
        }
    }
}
