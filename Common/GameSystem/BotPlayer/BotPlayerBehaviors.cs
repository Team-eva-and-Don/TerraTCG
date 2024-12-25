using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;

namespace TerraTCG.Common.GameSystem.BotPlayer
{
    internal partial class SimpleBotPlayer
    {

        // Get the best zone on my field to use a buff on,
        // depending on whether the buff is labeled as offensive or defensive
        private Zone GetBestBuffTarget(Card cardToUse)
        {
            if(cardToUse == null)
            {
                return null;
            }

            if(cardToUse.Role == ZoneRole.OFFENSE)
            {
                return GamePlayer.Field.Zones.Where(z => !z.IsEmpty())
                    .Where(z => cardToUse.ShouldTarget(z))
                    .Where(z => !cardToUse.SubTypes.Contains(CardSubtype.CONSUMABLE) || !z.PlacedCard.IsExerted)
                    .OrderByDescending(z => z.PlacedCard.GetAttackWithModifiers(z, null).Damage)
                    .FirstOrDefault();
            } else
            {
                return GamePlayer.Field.Zones.Where(z => !z.IsEmpty())
                    .Where(z => cardToUse.ShouldTarget(z))
                    .Where(z => z.PlacedCard.IsDamaged)
                    .OrderByDescending(z => z.PlacedCard.Template.MaxHealth - z.PlacedCard.CurrentHealth)
                    .FirstOrDefault();
            }
        }
        // Check whether any good candidates for attacking an enemy with a placed card exist
        // Return whether we decided to do an action
        private bool DecideAttack()
        {
            // While we have mana - choose the available attack with the highest damage and use it
            // against the enemy in the front row with the lowest health
            var bestAttackZone = GamePlayer.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => z.PlacedCard.GetAttackWithModifiers(z, null).Cost <= GamePlayer.Resources.Mana)
                .Where(z => !z.PlacedCard.IsExerted)
                .Where(z => z.Role == ZoneRole.OFFENSE)
                .OrderByDescending(z => z.PlacedCard.GetAttackWithModifiers(z, null).Damage)
                .FirstOrDefault();

            var action = new MoveCardOrAttackAction(bestAttackZone, GamePlayer);

            var bestTargetZone = GamePlayer.Opponent.Field.Zones.Where(z => !z.IsEmpty())
                .Where(action.CanAcceptZone)
                .OrderBy(z => z.PlacedCard.CurrentHealth)
                .FirstOrDefault();

            if(bestAttackZone != null && bestTargetZone != null)
            {
                DoMoveOrAttack(bestAttackZone, bestTargetZone);
                return true;
            }
            return false;
        }
        
        // Need custom logic for NPCs since they have unique activation conditions
        // Dryad
        private bool DecideRetreatCreature()
        {
            if(GamePlayer.Resources.TownsfolkMana == 0)
            {
                return false;
            }
            var dryadCard = GamePlayer.CreateCard<Dryad>().CardName;
            var cardInHand = GamePlayer.Hand.Cards.Where(c => c.CardName == dryadCard).FirstOrDefault();
            if(cardInHand == null)
            {
                return false;
            }

            var bestRetreatTarget = GamePlayer.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => new BounceCardAction(cardInHand, GamePlayer).CanAcceptZone(z))
                .Where(z => z.PlacedCard.CurrentHealth <= z.PlacedCard.Template.MaxHealth / 2)
                .FirstOrDefault();

            if(cardInHand != null && bestRetreatTarget != null)
            {
                UseTownsfolk(cardInHand, bestRetreatTarget);
                return true;
            }
            return false;
        }

        private bool DecideUseNonTargetingTownsfolk()
        {
            if(GamePlayer.Resources.TownsfolkMana == 0)
            {
                return false;
            }

            // TODO priority ordering of townsfolk cards
            var bestTownsfolk = GamePlayer.Hand.Cards
                .Where(c=>c.CardType == CardType.TOWNSFOLK)
                .Where(c=>c.SelectInHandAction(c, GamePlayer).CanAcceptActionButton())
                .FirstOrDefault();

            if(bestTownsfolk != null)
            {
                UseTownsfolk(bestTownsfolk);
                return true;
            }
            return false;
        }

        private bool DecideUseTargetingTownsfolk()
        {
            if(GamePlayer.Resources.TownsfolkMana == 0)
            {
                return false;
            }
            // Handlers with their own logic
            List<string> customHandlers = [
                GamePlayer.CreateCard<Dryad>().CardName,
                GamePlayer.CreateCard<OldMan>().CardName,
            ];

            // TODO priority ordering of townsfolk cards
            var bestTownsfolk = GamePlayer.Hand.Cards
                .Where(c=>c.CardType == CardType.TOWNSFOLK)
                .Where(c=>!customHandlers.Contains(c.CardName))
                .Where(c=>!c.SelectInHandAction(c, GamePlayer).CanAcceptActionButton())
                .FirstOrDefault();

            var bestBuffZone = GetBestBuffTarget(bestTownsfolk);

            if(bestTownsfolk != null && bestBuffZone != null)
            {
                UseTownsfolk(bestTownsfolk);
                return true;
            }
            return false;
        }

        private bool DecideMoveOpponent(ZoneRole srcRole, ZoneRole dstRole)
        {
            if(GamePlayer.Resources.TownsfolkMana == 0)
            {
                return false;
            }
            var oldManCard = GamePlayer.CreateCard<OldMan>().CardName;
            var cardInHand = GamePlayer.Hand.Cards.Where(c => c.CardName == oldManCard).FirstOrDefault();
            var possibleDamage = PossibleDamage;

            var bestMoveZone = GamePlayer.Opponent.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => z.Role == srcRole && z.PlacedCard.Template.Role == srcRole)
                .Where(z => z.PlacedCard.CurrentHealth <= possibleDamage)
                .OrderBy(z => z.PlacedCard.Template.MoveCost)
                .FirstOrDefault();

            var bestMoveToZone = GamePlayer.Opponent.Field.Zones.Where(z => z.IsEmpty())
                .Where(z => z.Role == dstRole)
                .FirstOrDefault();

            if(cardInHand != null && bestMoveZone != null && bestMoveToZone != null)
            {
                UseTownsfolk(cardInHand, bestMoveZone, bestMoveToZone);
                return true;
            }
            return false;
        }

        // Check whether any good candidates for placing a creature from hand exists
        private bool DecidePlayCreature()
        {
            var bestCardInHand = GamePlayer.Hand.Cards.Where(c => c.CardType == CardType.CREATURE)
                .Where(c => GamePlayer.Field.Zones.Any(z=>new DeployCreatureAction(c, GamePlayer).CanAcceptZone(z)))
                .OrderByDescending(c => c.Attacks[0].Damage)
                .FirstOrDefault();

            if(bestCardInHand == null)
            {
                return false;
            }

            var bestTargetZone = GamePlayer.Field.Zones
                .Where(z => new DeployCreatureAction(bestCardInHand, GamePlayer).CanAcceptZone(z))
                .Where(z => z.Role == (bestCardInHand?.Role ?? ZoneRole.OFFENSE))
                .FirstOrDefault();

            if(bestCardInHand != null && bestTargetZone != null)
            {
                PlaceCreature(bestCardInHand, bestTargetZone);
                return true;
            }
            return false;
        }

        // Check whether any good candidates for using an equipment exist
        private bool DecideUseItem()
        {
            var bestCardInHand = GamePlayer.Hand.Cards.Where(c => c.CardType == CardType.ITEM)
                .Where(c => c.Skills[0].Cost <= AvailableMana)
                .OrderByDescending(c => c.Priority)
                .FirstOrDefault();

            Zone bestTargetZone = GetBestBuffTarget(bestCardInHand);

            if(bestCardInHand != null && bestTargetZone != null)
            {
                UseItem(bestCardInHand, bestTargetZone);
                return true;
            }
            return false;

        }

        // Check whether any good candidate for moving a critter from the front row to the back row exists
        private bool DecideRetreatCritter()
        {
            var bestRetreatZone = GamePlayer.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => z.PlacedCard.Template.Role == ZoneRole.DEFENSE)
                .Where(z => !z.IsBlocked())
                .Where(z => z.PlacedCard.Template.MoveCost <= AvailableMana)
                .OrderBy(z => z.PlacedCard.CurrentHealth)
                .FirstOrDefault();

            var bestDestZone = GamePlayer.Field.Zones.Where(z => z.IsEmpty())
                .Where(z => z.Role == ZoneRole.DEFENSE && z.IsBlocked())
                .OrderBy(z => z.IsBlocked() ? 0 : 1)
                .FirstOrDefault();

            if(bestRetreatZone != null && bestDestZone != null)
            {
                DoMoveOrAttack(bestRetreatZone, bestDestZone);
                return true;
            }
            return false;
        }

        // Check whether any good candidate for moving an attacker from the back row to the front row exists
        private bool DecideAdvanceAttacker()
        {
            var bestAdvanceZone = GamePlayer.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => z.PlacedCard.Template.Role == ZoneRole.OFFENSE)
                .Where(z => z.Role == ZoneRole.DEFENSE)
                .Where(z => z.PlacedCard.Template.MoveCost <= AvailableMana)
                .OrderBy(z =>z.PlacedCard.GetAttackWithModifiers(z, null).Damage)
                .FirstOrDefault();

            var bestDestZone = GamePlayer.Field.Zones.Where(z => z.IsEmpty())
                .Where(z => z.Role == ZoneRole.OFFENSE)
                .FirstOrDefault();

            if(bestAdvanceZone != null && bestDestZone != null)
            {
                DoMoveOrAttack(bestAdvanceZone, bestDestZone);
                return true;
            }
            return false;
        }

        // Use any available critter skills
        private bool DecideUseUtilitySkill()
        {
            var bestSkillZone = GamePlayer.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => !z.PlacedCard.IsExerted && z.PlacedCard.Template.HasSkill)
                .Where(z => z.PlacedCard.Template.Skills[0].Cost <= AvailableMana)
                .Where(z => z.PlacedCard.Template.Skills[0].SkillType == ActionType.SKILL)
                .OrderBy(z => z.PlacedCard.Template.Skills[0].Cost)
                .FirstOrDefault();

            if(bestSkillZone != null)
            {
                DoUseSkill(bestSkillZone);
                return true;
            }
            return false;
        }

        // Use any available critter skills
        private bool DecideUseTargetedSkill()
        {
            var bestSkillZone = GamePlayer.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => !z.PlacedCard.IsExerted && z.PlacedCard.Template.HasSkill)
                .Where(z => z.PlacedCard.Template.Skills[0].Cost <= AvailableMana)
                .Where(z => z.PlacedCard.Template.Skills[0].SkillType == ActionType.TARGET_ALLY)
                .OrderBy(z => z.PlacedCard.Template.Skills[0].Cost)
                .FirstOrDefault();
            if(bestSkillZone == null)
            {
                return false;
            }
            var skill = bestSkillZone.PlacedCard.Template.Skills[0];
            var action = new MoveCardOrAttackAction(bestSkillZone, GamePlayer);
            action.AcceptActionButton();

            Zone bestTargetZone;
            if(skill.Role == ZoneRole.OFFENSE)
            {
                bestTargetZone = GamePlayer.Field.Zones.Where(z => !z.IsEmpty() && !z.PlacedCard.IsExerted)
                    .Where(action.CanAcceptZone)
                    .OrderByDescending(z => z.PlacedCard.GetAttackWithModifiers(z, null).Damage)
                    .FirstOrDefault();
            } else
            {
                bestTargetZone = GamePlayer.Field.Zones.Where(z => !z.IsEmpty() && z.PlacedCard.IsDamaged)
                    .Where(action.CanAcceptZone)
                    .OrderByDescending(z => z.PlacedCard.Template.MaxHealth - z.PlacedCard.CurrentHealth)
                    .FirstOrDefault();
            }

            if(bestSkillZone != null && bestTargetZone != null)
            {
                DoUseTargetedSkill(bestSkillZone, bestTargetZone);
                return true;
            }
            return false;
        }

    }
}
