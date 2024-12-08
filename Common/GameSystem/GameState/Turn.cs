using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal class Turn
    {
        internal Game Game { get; set; }
        internal GamePlayer ActivePlayer { get; set; }
        internal int TurnCount { get; set; }

        internal List<Action> ActionLog { get; set; } = [];

        public void Start()
        {
            if(ActivePlayer.ManaPerTurn < GamePlayer.MAX_MANA)
            {
                ActivePlayer.ManaPerTurn += 1;
            }
            ActivePlayer.Resources = new(
                ActivePlayer.Resources.Health,
                ActivePlayer.ManaPerTurn,
                1
            );
        }

        public void End()
        {
            Game.CurrentTurn = new()
            {
                Game = Game,
                ActivePlayer = ActivePlayer.Opponent,
                TurnCount = TurnCount + 1
            };
            Game.CurrentTurn.Start();
        }
    }
}
