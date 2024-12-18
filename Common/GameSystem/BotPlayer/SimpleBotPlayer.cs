using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;

namespace TerraTCG.Common.GameSystem.BotPlayer
{
    internal class SimpleBotPlayer : IBotPlayer
    {
        public Game Game { get; private set; }
        public GamePlayer Player { get; private set; }

        private TimeSpan PauseBetweenActions = TimeSpan.FromSeconds(0.25f);
        private TimeSpan LastBusyTime = TimeSpan.FromSeconds(0f);

        public void SetGame(Game game, GamePlayer player)
        {
            Game = game;
            Player = player;
        }

        private void DoMoveOrAttack(Zone sourceZone, Zone destZone)
        {
            var action = new MoveCardOrAttackAction(sourceZone, Player);
            Player.InProgressAction = action;
            action.AcceptZone(destZone);
            action.Complete();
        }

        private void PlaceCreature(Card sourceCard, Zone destZone)
        {
            Main.NewText($"Bot says: Using {sourceCard.Name}!");
            var action = new DeployCardAction(sourceCard, Player);
            Player.InProgressAction = action;
            action.AcceptZone(destZone);
            action.Complete();
        }

        private void EquipItem(Card sourceCard, Zone destZone)
        {
            Main.NewText($"Bot says: Using {sourceCard.Name}!");
            var action = new ApplyModifierAction(sourceCard, Player);
            Player.InProgressAction = action;
            action.AcceptZone(destZone);
            action.Complete();
        }

        private void UseTownsfolk(Card sourceCard, Zone destZone)
        {
            Main.NewText($"Bot says: Using {sourceCard.Name}!");
            var action = sourceCard.SelectInHandAction(sourceCard, Player);
            Player.InProgressAction = action;
            action.AcceptZone(destZone);
            action.Complete();
        }

        private void UseTownsfolk(Card sourceCard, Zone destZone1, Zone destZone2)
        {
            Main.NewText($"Bot says: Using {sourceCard.Name}!");
            var action = sourceCard.SelectInHandAction(sourceCard, Player);
            Player.InProgressAction = action;
            action.AcceptZone(destZone1);
            action.AcceptZone(destZone2);
            action.Complete();
        }

        // Check whether any good candidates for attacking an enemy with a placed card exist
        // Return whether we decided to do an action
        private bool DecideAttack()
        {
            // While we have mana - choose the available attack with the highest damage and use it
            // against the enemy in the front row with the lowest health
            var bestAttackZone = Player.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => z.PlacedCard.Template.Attacks[0].Cost <= Player.Resources.Mana)
                .Where(z => !z.PlacedCard.IsExerted)
                .Where(z => z.Role == ZoneRole.OFFENSE)
                .OrderByDescending(z => z.PlacedCard.GetAttackWithModifiers(z, null).Damage)
                .FirstOrDefault();

            var bestTargetZone = Player.Opponent.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => z.Role == ZoneRole.OFFENSE)
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
            if(Player.Resources.TownsfolkMana == 0)
            {
                return false;
            }
            var dryadCard = Dryad.Instance.CreateCard().CardName;
            var cardInHand = Player.Hand.Cards.Where(c => c.CardName == dryadCard).FirstOrDefault();

            var bestRetreatTarget = Player.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => z.PlacedCard.CurrentHealth <= z.PlacedCard.Template.MaxHealth / 2)
                .FirstOrDefault();

            if(cardInHand != null && bestRetreatTarget != null)
            {
                UseTownsfolk(cardInHand, bestRetreatTarget);
                return true;
            }
            return false;
        }

        private bool DecideMoveOpponent(ZoneRole srcRole, ZoneRole dstRole)
        {
            if(Player.Resources.TownsfolkMana == 0)
            {
                return false;
            }
            var oldManCard = OldMan.Instance.CreateCard().CardName;
            var cardInHand = Player.Hand.Cards.Where(c => c.CardName == oldManCard).FirstOrDefault();

            var bestMoveZone = Player.Opponent.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => z.Role == srcRole && z.PlacedCard.Template.Role == srcRole)
                .OrderBy(z => z.PlacedCard.Template.MoveCost)
                .FirstOrDefault();

            var bestMoveToZone = Player.Opponent.Field.Zones.Where(z => z.IsEmpty())
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
            var bestCardInHand = Player.Hand.Cards.Where(c => c.CardType == CardType.CREATURE)
                .OrderByDescending(c => c.Attacks[0].Damage)
                .FirstOrDefault();

            var bestTargetZone = Player.Field.Zones.Where(z => z.IsEmpty())
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
        private bool DecideEquipItem()
        {
            var bestCardInHand = Player.Hand.Cards.Where(c => c.CardType == CardType.ITEM)
                .Where(c => c.SubTypes.Contains(CardSubtype.EQUIPMENT))
                .Where(c => c.Skills[0].Cost <= Player.Resources.Mana)
                .OrderByDescending(c => c.Priority)
                .FirstOrDefault();

            var bestTargetZone = Player.Field.Zones.Where(z => !z.IsEmpty())
                .OrderByDescending(z => z.PlacedCard.GetAttackWithModifiers(z, null).Damage)
                .FirstOrDefault();

            if(bestCardInHand != null && bestTargetZone != null)
            {
                EquipItem(bestCardInHand, bestTargetZone);
                return true;
            }
            return false;

        }

        // Check whether any good candidate for moving a critter from the front row to the back row exists
        private bool DecideRetreatCritter()
        {
            var bestRetreatZone = Player.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => z.PlacedCard.Template.Role == ZoneRole.DEFENSE)
                .Where(z => z.Role == ZoneRole.OFFENSE)
                .Where(z => z.PlacedCard.Template.MoveCost <= Player.Resources.Mana)
                .OrderBy(z => z.PlacedCard.CurrentHealth)
                .FirstOrDefault();

            var bestDestZone = Player.Field.Zones.Where(z => z.IsEmpty())
                .Where(z => z.Role == ZoneRole.DEFENSE)
                .FirstOrDefault();

            if(bestRetreatZone != null && bestDestZone != null)
            {
                DoMoveOrAttack(bestRetreatZone, bestDestZone);
                return true;
            }
            return false;
        }

        // Check whether any good candidate for moving an attacker from the back row to the fromt row exists
        private bool DecideAdvanceAttacker()
        {
            var bestAdvanceZone = Player.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => z.PlacedCard.Template.Role == ZoneRole.OFFENSE)
                .Where(z => z.Role == ZoneRole.DEFENSE)
                .Where(z => z.PlacedCard.Template.MoveCost <= Player.Resources.Mana)
                .OrderBy(z =>z.PlacedCard.GetAttackWithModifiers(z, null).Damage)
                .FirstOrDefault();

            var bestDestZone = Player.Field.Zones.Where(z => z.IsEmpty())
                .Where(z => z.Role == ZoneRole.OFFENSE)
                .FirstOrDefault();

            if(bestAdvanceZone != null && bestDestZone != null)
            {
                DoMoveOrAttack(bestAdvanceZone, bestDestZone);
                return true;
            }
            return false;
        }

        public void Update()
        {
            if (!Player.IsMyTurn || Game.IsDoingAnimation())
            {
                LastBusyTime = Main._drawInterfaceGameTime.TotalGameTime;
                return;
            } else if (Main._drawInterfaceGameTime.TotalGameTime < LastBusyTime + PauseBetweenActions)
            {
                return;
            }

            if(DecideRetreatCreature()) return;

            if(DecideMoveOpponent(ZoneRole.DEFENSE, ZoneRole.OFFENSE)) return;

            if(DecidePlayCreature()) return;

            if(DecideRetreatCritter()) return;

            if(DecideEquipItem()) return;

            if(DecideAttack()) return;

            if(DecideAdvanceAttacker()) return;

            Player.PassTurn();
        }
    }
}
