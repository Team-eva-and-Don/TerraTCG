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
    internal class ApplyModifierAction(Card card, GamePlayer player) : IGameAction
    {
        private Zone zone;

        public bool CanAcceptZone(Zone zone) => player.Owns(zone) && !zone.IsEmpty() && player.Resources.Mana >= card.Skills[0].Cost;

        public bool AcceptZone(Zone zone)
        {
            this.zone = zone;
            return true;
        }

        public void Complete()
        {
            var showAnimation = new ShowCardAnimation(TCGPlayer.TotalGameTime, card, zone);
            player.Game.FieldAnimation = showAnimation;
            var duration = showAnimation.Duration;

            zone.Animation = new DelayAnimation(duration, zone, zone.PlacedCard, t =>
                new ApplyModifierAnimation(zone, card.Modifiers, t));

            zone.PlacedCard.CardModifiers.AddRange(card.Modifiers);
            player.Resources = player.Resources.UseResource(mana: card.Skills[0].Cost);
            player.Hand.Remove(card);
        }
    }
}
