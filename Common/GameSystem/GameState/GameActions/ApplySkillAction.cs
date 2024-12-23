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
    internal class ApplySkillAction(Card card, GamePlayer player) : IGameAction
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

            zone.QueueAnimation(new IdleAnimation(zone.PlacedCard, duration: duration));
            zone.QueueAnimation(new ApplyModifierAnimation(zone.PlacedCard, card.Skills[0].Texture));

            card.Skills[0].DoSkill(player, null, zone);
            player.Resources = player.Resources.UseResource(mana: card.Skills[0].Cost);
            player.Hand.Remove(card);
        }
    }
}
