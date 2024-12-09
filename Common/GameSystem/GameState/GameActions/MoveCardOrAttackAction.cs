using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing.Animations;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    // Action to either move a card to a neighboring zone or attack against an enemy creature
    // TODO we probably want to split those out, UI-permitting
    internal class MoveCardOrAttackAction(Zone startZone, GamePlayer player) : IGameAction
    {
        private Zone endZone;

        private ActionType actionType = ActionType.NONE;

        public bool CanAcceptCardInHand(Card card) => false;

        public bool CanAcceptZone(Zone zone) 
        { 
            if(player.Owns(zone) && zone.IsEmpty())
            {
                return startZone.PlacedCard.Template.MoveCost <= player.Resources.Mana;

            } else if (!player.Owns(zone) && !zone.IsEmpty())
            {
                return startZone.PlacedCard.Template.Attacks[0].Cost <= player.Resources.Mana;
            } else
            {
                return false;
            }
        }

        public bool AcceptCardInHand(Card card) => false;

        public bool CanAcceptActionButton(ActionType actionType)
        {
            return startZone.PlacedCard.Template.HasSkill &&
                startZone.PlacedCard.Template.Skills[0].Cost <= player.Resources.Mana;
        }

        public bool AcceptZone(Zone zone)
        {
            endZone = zone;
            return true;
        }

        public bool AcceptActionButton(ActionType actionType)
        {
            this.actionType = actionType;
            return true;
        }

        private void DoMove()
        {
            // move within own field
            endZone.PlacedCard = startZone.PlacedCard;
            startZone.PlacedCard = null;
            startZone.Animation = new RemoveCardAnimation(startZone, endZone.PlacedCard, Main._drawInterfaceGameTime.TotalGameTime);
            endZone.Animation = new PlaceCardAnimation(endZone, Main._drawInterfaceGameTime.TotalGameTime);
            player.Resources = player.Resources.UseResource(mana: endZone.PlacedCard.Template.MoveCost);
        }

        private void DoAttack()
        {
            var currTime = Main._drawInterfaceGameTime.TotalGameTime;
            // attack opposing field
            var prevHealth = endZone.PlacedCard.CurrentHealth;
            var attack = startZone.PlacedCard.Template.Attacks[0];
            attack.DoAttack(attack, startZone, endZone);

            player.Resources = player.Resources.UseResource(mana: startZone.PlacedCard.Template.Attacks[0].Cost);

            startZone.Animation = new MeleeAttackAnimation(startZone, endZone, currTime);
            if(endZone.PlacedCard.CurrentHealth > 0)
            {
                endZone.Animation = new TakeDamageAnimation(endZone, currTime, TimeSpan.FromSeconds(0.5f), prevHealth, endZone.Animation.StartTime);
            } else
            {
                endZone.Animation = new DeathAnimation(
                    endZone, currTime, TimeSpan.FromSeconds(0.5f), prevHealth, endZone.PlacedCard, endZone.Animation.StartTime);
                endZone.Owner.Resources = endZone.Owner.Resources.UseResource(health: 1);
                endZone.PlacedCard = null;
            }
        }

        private void DoSkill()
        {
            var skill = startZone.PlacedCard.Template.Skills[0];
            player.Resources = player.Resources.UseResource(mana: skill.Cost);
            skill.DoSkill(player, startZone);
        }

        public void Complete()
        {
            if(actionType != ActionType.NONE)
            {
                DoSkill();
            } else if (player.Owns(endZone))
            {
                DoMove();
            } else  
            {
                DoAttack();
            }
        }

        public void Cancel()
        {
            // No-op
        }
    }
}
