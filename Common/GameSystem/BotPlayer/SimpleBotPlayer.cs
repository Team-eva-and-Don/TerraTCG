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

        private bool HasAttacked { get; set; }

        private int AvailableMana => HasAttacked ? GamePlayer.Resources.Mana : GamePlayer.Resources.Mana - ReservedAttackMana;

        // Pre-calculate all the damage we can do this turn, used for
        // certain decisions (eg. whether to force-promote a weakened unit)
        private int PossibleDamage
        {
            get
            {
                var possibleDmg = 0;
                var availableMana = GamePlayer.Resources.Mana;
                var sortedAttacks = GamePlayer.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => !z.PlacedCard.IsExerted)
                .Where(z => z.Role == ZoneRole.OFFENSE)
                .Select(z=>z.PlacedCard.GetAttackWithModifiers(z, null))
                .Where(a => a.Cost <= GamePlayer.Resources.Mana)
                .OrderByDescending(a => a.Damage);

                foreach(var a in sortedAttacks)
                {
                    if(availableMana >= a.Damage)
                    {
                        possibleDmg += a.Damage;
                        availableMana -= a.Cost;
                    }
                }
                return possibleDmg;
            }
        }

        private void CalculateReservedAttackMana()
        {
            if(ReservedAttackMana > 0 || HasAttacked)
            {
                return;
            }
            var bestAttack = GamePlayer.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => z.PlacedCard.GetAttackWithModifiers(z, null).Cost <= GamePlayer.Resources.Mana)
                .Where(z => !z.PlacedCard.IsExerted)
                .Where(z => z.Role == ZoneRole.OFFENSE)
                .Select(z=>z.PlacedCard.GetAttackWithModifiers(z, null))
                .OrderByDescending(a => a.ManaEfficiency)
                .FirstOrDefault();
            ReservedAttackMana = bestAttack.Cost;
        }


        private void DoMoveOrAttack(Zone sourceZone, Zone destZone)
        {
            HasAttacked = true;
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

            if(DecideAttack()) return;

            if(DecideAdvanceAttacker()) return;

            if(DecideRetreatCreature()) return;

            ResetStateAndPassTurn();
        }

        private void ResetStateAndPassTurn()
        {
            HasAttacked = false;
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
