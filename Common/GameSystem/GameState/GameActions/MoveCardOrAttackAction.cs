using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    // "Do everything" action that encompasses any game action initiated by clicking an on-field card.
    // - Declare an attack by clicking an ally zone, then any enemy zone
    // - Move a creature by clicking two ally zones
    // - Use a skill by clicking an ally zone, then a "use skill" button
    internal class MoveCardOrAttackAction(Zone startZone, GamePlayer player) : IGameAction
    {
        private Zone endZone;

        private ActionType actionType = ActionType.DEFAULT;

        public bool CanAcceptCardInHand(Card card) => false;

        public bool CanAcceptZone(Zone zone) 
        { 
            if(startZone?.PlacedCard?.IsExerted ?? true)
            {
                return false;
            }

            if(actionType == ActionType.DEFAULT && player.Owns(zone) && zone.IsEmpty())
            {
                return startZone.PlacedCard.Template.MoveCost <= player.Resources.Mana;

            } else if (actionType == ActionType.DEFAULT && !player.Owns(zone) && !zone.IsEmpty())
            {
                return CanAttackZone(zone);
            } else if (actionType == ActionType.TARGET_ALLY && player.Owns(zone) && !zone.IsEmpty())
            {
                return true;
            } else
            {
                return false;
            }
        }

        private bool CanAttackZone(Zone zone)
        {
            return startZone.HasPlacedCard() && startZone.PlacedCard.GetAttackWithModifiers(startZone, zone).Cost <= player.Resources.Mana &&
                startZone.PlacedCard.GetValidAttackZones(startZone, zone).Contains(zone);
        }

        public bool AcceptCardInHand(Card card) => false;

        public bool CanAcceptActionButton()
        {
            if(startZone.PlacedCard?.GetSkillWithModifiers(startZone, null) is Skill skill)
            {
                return startZone.HasPlacedCard() &&  actionType == ActionType.DEFAULT && startZone.PlacedCard.Template.HasSkillText &&
                    skill.Cost <= player.Resources.Mana &&
                    !startZone.PlacedCard.IsExerted;
            }
            return false;
        }

        public bool AcceptZone(Zone zone)
        {
            endZone = zone;
            return true;
        }

        public bool AcceptActionButton()
        {
            actionType = startZone.PlacedCard.Template.Skills[0].SkillType;
            return actionType == ActionType.SKILL;
        }

        private void DoMove()
        {
            // move within own field
            endZone.PlacedCard = startZone.PlacedCard;
            startZone.PlacedCard = null;
            startZone.QueueAnimation(new RemoveCardAnimation(endZone.PlacedCard));
            endZone.QueueAnimation(new PlaceCardAnimation(endZone.PlacedCard));
            player.Resources = player.Resources.UseResource(mana: endZone.PlacedCard.Template.MoveCost);
        }

        private void DoAttack()
        {
            var currTime = TCGPlayer.TotalGameTime;
            // attack opposing field
            var prevHealth = endZone.PlacedCard.CurrentHealth;
            startZone.PlacedCard.IsExerted = true;
            var attack = startZone.PlacedCard.GetAttackWithModifiers(startZone, endZone);
            player.Resources = player.Resources.UseResource(mana: attack.Cost);
            attack.DoAttack(attack, startZone, endZone);

            startZone.QueueAnimation(new MeleeAttackAnimation(startZone.PlacedCard, endZone));
            endZone.QueueAnimation(new IdleAnimation(endZone.PlacedCard, TimeSpan.FromSeconds(0.5f), prevHealth));
            endZone.QueueAnimation(new TakeDamageAnimation(endZone.PlacedCard, prevHealth));

            player.Field.ClearModifiers(player, startZone, GameEvent.AFTER_ATTACK);
            player.Opponent.Field.ClearModifiers(player, endZone, GameEvent.AFTER_RECEIVE_ATTACK);


            if(endZone.PlacedCard.CurrentHealth <= 0)
            {
                endZone.QueueAnimation(new RemoveCardAnimation(endZone.PlacedCard));
                endZone.Owner.Resources = endZone.Owner.Resources.UseResource(health: 1);
                endZone.PlacedCard = null;
            }

            // both cards can die during an attack exchange
            if(startZone.PlacedCard.CurrentHealth <= 0)
            {
                startZone.QueueAnimation(new RemoveCardAnimation(startZone.PlacedCard));
                startZone.Owner.Resources = startZone.Owner.Resources.UseResource(health: 1);
                startZone.PlacedCard = null;
            }
        }

        private void DoSkill()
        {
            var skill = startZone.PlacedCard.GetSkillWithModifiers(startZone, null);
            startZone.PlacedCard.IsExerted = true;
            player.Resources = player.Resources.UseResource(mana: skill.Cost);
            skill.DoSkill(player, startZone, endZone);
            startZone.QueueAnimation(new ActionAnimation(startZone.PlacedCard));
            endZone?.QueueAnimation(new ActionAnimation(endZone.PlacedCard));
        }

        public void Complete()
        {
            if(actionType != ActionType.DEFAULT)
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

        public Color HighlightColor(Zone zone)
        {
            if(actionType == ActionType.DEFAULT)
            {
                return TCGPlayer.LocalGamePlayer.Owns(zone) ? Color.LightSkyBlue : Color.LightCoral;
            } else
            {
                return Color.Goldenrod;
            }

        }

        public void Cancel()
        {
            // No-op
        }
    }
}
