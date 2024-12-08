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
        internal const int MAX_MANA_PER_TURN = 8;

        internal Game Game { get; set; }
        internal GamePlayer ActivePlayer { get; set; }
        internal int TurnCount { get; set; }

        internal List<Action> ActionLog { get; set; } = [];

        public void Start()
        {
            if(ActivePlayer.ManaPerTurn < MAX_MANA_PER_TURN)
            {
                ActivePlayer.ManaPerTurn += 1;
            }
            ActivePlayer.Mana = ActivePlayer.ManaPerTurn;
            ActivePlayer.TownsfolkMana = 1;
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
