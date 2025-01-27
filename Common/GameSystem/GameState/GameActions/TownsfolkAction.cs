using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations;
using static TerraTCG.Common.GameSystem.GameState.GameActions.IGameAction;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    internal abstract class TownsfolkAction(Card card, GamePlayer player) : IGameAction
    {
        internal Card Card { get; } = card;
        internal GamePlayer Player { get; } = player;

        private bool checkingValidZone = false;
        public virtual bool CanAcceptZone(Zone zone) => checkingValidZone || Player.Resources.TownsfolkMana > 0;
        public abstract bool AcceptZone(Zone zone);

        public abstract ActionLogInfo GetLogMessage();

        public virtual Zone TargetZone() => null;

        public TimeSpan GetAnimationStartDelay() => ShowCardAnimation.DURATION;
        // Player == TCGPlayer.LocalGamePlayer ? TimeSpan.FromSeconds(0f) : ShowCardAnimation.DURATION;

        public string GetActionButtonTooltip()
        {
            return $"{ActionText("Use")} {Card.CardName}";
        }

        public virtual string GetZoneTooltip(Zone zone)
        {
            return $"{ActionText("Use")} {Card.CardName} {ActionText("On")} {zone.CardName}";
        }

        // TODO this is hacky, check whether not having a townsfolk emblem
        // is the reason that the zone can't be selected
        public string GetCantAcceptZoneTooltip(Zone zone) {
            if(Player.Resources.TownsfolkMana == 0)
            {
                // TODO this is hacky
                checkingValidZone = true;
                var couldUseIfHadMana = CanAcceptZone(zone);
                checkingValidZone = false;
                if (couldUseIfHadMana)
                {
                    return ActionText("NotEnoughTownsfolk");
                }
            }
            return null;
        }

        public virtual void Complete()
        {
            Player.Resources = Player.Resources.UseResource(townsfolkMana: 1);
            Player.Hand.Remove(Card);
			Player.Game.FieldAnimation = new ShowCardAnimation(TCGPlayer.TotalGameTime, Card, TargetZone(), Player == TCGPlayer.LocalGamePlayer);
        }
    }
}
