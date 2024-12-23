using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;

namespace TerraTCG.Common.GameSystem.BotPlayer
{
    internal partial class SimpleBotPlayer : IBotPlayer
    {
        public CardGame Game { get; private set; }
        public GamePlayer GamePlayer { get; set; }

        private readonly TimeSpan PauseBetweenActions = TimeSpan.FromSeconds(0.5f);
        private TimeSpan LastBusyTime = TimeSpan.FromSeconds(0f);

        private int ReservedAttackMana { get; set; }

        private int AvailableMana => GamePlayer.Resources.Mana - ReservedAttackMana;

        // Pre-calculate all the damage we can do this turn, used for
        // certain decisions (eg. whether to force-promote a weakened unit)
        private int PossibleDamage;
        private (int, int) GetPossibleDamageAndCost()
        {
            var possibleDmg = 0;
            var totalCost = 0;
            var availableMana = GamePlayer.Resources.Mana;
            var sortedAttacks = GamePlayer.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => !z.PlacedCard.IsExerted)
                .Where(z => z.Role == ZoneRole.OFFENSE)
                .Select(z=>z.PlacedCard.GetAttackWithModifiers(z, null))
                .Where(a => a.Cost <= GamePlayer.Resources.Mana)
                .OrderByDescending(a => a.ManaEfficiency);

            foreach(var a in sortedAttacks)
            {
                if(availableMana >= totalCost + a.Cost)
                {
                    possibleDmg += a.Damage;
                    totalCost += a.Cost;
                }
            }
            return (possibleDmg, totalCost);
        }

        private void CalculateReservedAttackMana()
        {
            var attackInfo = GetPossibleDamageAndCost();
            PossibleDamage = attackInfo.Item1;
            if(ReservedAttackMana > 0)
            {
                return;
            }
            // Every other turn, dedicate all mana to attacking
            // This is a little wonky but the bot won't always use
            // its other cards otherwise
            if(Game.CurrentTurn.TurnCount % 2 == 0)
            {
                ReservedAttackMana = 0;
            } else
            {
                ReservedAttackMana = attackInfo.Item2;
            }
        }


        private void DoMoveOrAttack(Zone sourceZone, Zone destZone)
        {
            var action = new MoveCardOrAttackAction(sourceZone, GamePlayer);
            GamePlayer.InProgressAction = action;
            action.AcceptZone(destZone);
            action.Complete();
        }

        private void DoUseSkill(Zone sourceZone)
        {
            var action = new MoveCardOrAttackAction(sourceZone, GamePlayer);
            GamePlayer.InProgressAction = action;
            action.AcceptActionButton();
            action.Complete();
        }

        private void DoUseTargetedSkill(Zone sourceZone, Zone destZone)
        {
            var action = new MoveCardOrAttackAction(sourceZone, GamePlayer);
            GamePlayer.InProgressAction = action;
            action.AcceptActionButton();
            action.AcceptZone(destZone);
            action.Complete();
        }

        private void PlaceCreature(Card sourceCard, Zone destZone)
        {
            var action = new DeployCardAction(sourceCard, GamePlayer);
            GamePlayer.InProgressAction = action;
            action.AcceptZone(destZone);
            action.Complete();
        }

        private void EquipItem(Card sourceCard, Zone destZone)
        {
            var action = new ApplyModifierAction(sourceCard, GamePlayer);
            GamePlayer.InProgressAction = action;
            action.AcceptZone(destZone);
            action.Complete();
        }

        private void UseTownsfolk(Card sourceCard)
        {
            var action = sourceCard.SelectInHandAction(sourceCard, GamePlayer);
            GamePlayer.InProgressAction = action;
            action.AcceptActionButton();
            action.Complete();
        }

        private void UseTownsfolk(Card sourceCard, Zone destZone)
        {
            var action = sourceCard.SelectInHandAction(sourceCard, GamePlayer);
            GamePlayer.InProgressAction = action;
            action.AcceptZone(destZone);
            action.Complete();
        }

        private void UseTownsfolk(Card sourceCard, Zone destZone1, Zone destZone2)
        {
            var action = sourceCard.SelectInHandAction(sourceCard, GamePlayer);
            GamePlayer.InProgressAction = action;
            action.AcceptZone(destZone1);
            action.AcceptZone(destZone2);
            action.Complete();
        }

        public void Update()
        {
            if (!GamePlayer.IsMyTurn || Game.IsDoingAnimation())
            {
                LastBusyTime = TCGPlayer.TotalGameTime;
                return;
            } else if (TCGPlayer.TotalGameTime < LastBusyTime + PauseBetweenActions)
            {
                return;
            }
            CalculateReservedAttackMana();

            if(DecideMoveOpponent(ZoneRole.DEFENSE, ZoneRole.OFFENSE)) return;

            if(DecidePlayCreature()) return;

            if(DecideUseNonTargetingTownsfolk()) return;

            if(DecideRetreatCritter()) return;

            if(DecideUseItem()) return;

            if(DecideUseUtilitySkill()) return;

            if(DecideUseTargetedSkill()) return;

            if(DecideAdvanceAttacker()) return;

            if(DecideRetreatCreature()) return;

            if(DecideAttack()) return;

            ResetStateAndPassTurn();
        }

        private void ResetStateAndPassTurn()
        {
            PossibleDamage = 0;
            ReservedAttackMana = 0;
            GamePlayer.PassTurn();
        }

        public void StartGame(GamePlayer player, CardGame game)
        {
            Game = game;
            GamePlayer = player;
            BotPlayerSystem.Instance.RegisterBotPlayer(this, game, player);
        }

        public void EndGame()
        {
            // de-register myself
            ModContent.GetInstance<BotPlayerSystem>().UnregisterBotPlayer(this);
        }
    }
}
