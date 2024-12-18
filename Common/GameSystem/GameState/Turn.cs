using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations;

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

            if(TurnCount > 1 && !ActivePlayer.Deck.Empty())
            {
                ActivePlayer.Hand.Add(ActivePlayer.Deck.Draw());
            }
            foreach(var zone in ActivePlayer.Field.Zones.Where(z=>!z.IsEmpty()))
            {
                if(zone.PlacedCard.IsExerted)
                {
                    zone.Animation = new BecomeActiveAnimation(zone, Main._drawInterfaceGameTime.TotalGameTime);
                }
                zone.PlacedCard.IsExerted = false;
            }

            // Don't draw on the first turn

            TCGPlayer.LocalGamePlayer.Game.FieldAnimation =
                new TurnChangeAnimation(Main._drawInterfaceGameTime.TotalGameTime, this);
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
