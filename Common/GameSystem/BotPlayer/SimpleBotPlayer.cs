using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;

namespace TerraTCG.Common.GameSystem.BotPlayer
{
    internal class SimpleBotPlayer : IBotPlayer
    {
        public Game Game { get; private set; }
        public GamePlayer Player { get; private set; }

        public void SetGame(Game game, GamePlayer player)
        {
            Game = game;
            Player = player;
        }

        private void DoAttack(Zone sourceZone, Zone destZone)
        {
            var action = new MoveCardOrAttackAction(sourceZone, Player);
            Player.InProgressAction = action;
            action.AcceptZone(destZone);
            action.Complete();
        }

        public void Update()
        {
            if (!Player.IsMyTurn || Game.IsDoingAnimation())
            {
                return;
            }

            // While we have mana - choose the available attack with the highest mana and use it
            var bestAttackZone = Player.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => z.PlacedCard.Template.Attacks[0].Cost <= Player.Resources.Mana)
                .Where(z => z.Role == ZoneRole.OFFENSE)
                .OrderByDescending(z => z.PlacedCard.Template.Attacks[0].Damage)
                .FirstOrDefault();

            var bestTargetZone = Player.Opponent.Field.Zones.Where(z => !z.IsEmpty())
                .Where(z => z.Role == ZoneRole.OFFENSE)
                .OrderBy(z => z.PlacedCard.CurrentHealth)
                .FirstOrDefault();

            if(bestAttackZone != null && bestTargetZone != null)
            {
                DoAttack(bestAttackZone, bestTargetZone);
            } else
            {
                Player.PassTurn();
            }
        }
    }
}
