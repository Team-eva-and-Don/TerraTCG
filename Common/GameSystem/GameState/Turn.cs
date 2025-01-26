using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal class Turn
    {
        internal CardGame Game { get; set; }
        internal GamePlayer ActivePlayer { get; set; }
        internal int TurnCount { get; set; }

        // Certain cards care about item usage count
        internal int UsedItemCount { get; set; }
        public List<ActionLogInfo> ActionLog { get; internal set; } = [];

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


            // Don't draw on the first turn
            if(TurnCount > 2 && !ActivePlayer.Deck.Empty())
            {
                ActivePlayer.Hand.Add(ActivePlayer.Deck.Draw());
            }

            foreach(var zone in ActivePlayer.Field.Zones.Where(z=>!z.IsEmpty()))
            {
                if(zone.PlacedCard.IsExerted && (zone.Animation?.IsDefault() ?? true))
                {
                    zone.QueueAnimation(new BecomeActiveAnimation(zone.PlacedCard));
                }
                zone.PlacedCard.IsExerted = false;
            }

            foreach(var zone in ActivePlayer.Game.AllZones())
            {
                zone.Owner.Field.ClearModifiers(ActivePlayer, zone, GameEvent.START_TURN);
            }

			foreach(var zone in ActivePlayer.Game.AllZones())
			{
				var fieldModifiers = zone.PlacedCard?.Template.FieldModifiers?.Invoke();
				if(fieldModifiers?.Count > 0)
				{
					zone.Owner.Field.CardModifiers.AddRange(fieldModifiers);
				}
			}

            TCGPlayer.LocalGamePlayer.Game.FieldAnimation =
                new TurnChangeAnimation(TCGPlayer.TotalGameTime, this);

            ActionLog = [new(null, $"Start of Turn {TurnCount}")];
        }

        public void End()
        {
            foreach(var zone in ActivePlayer.Game.AllZones())
            {
                zone.Owner.Field.ClearModifiers(ActivePlayer, zone, GameEvent.END_TURN);
            }
            // If the turn player was unable to keep a creature on board this turn,
            // end the game
            if(!ActivePlayer.Field.Zones.Any(z=>z.HasPlacedCard()))
            {
                ActivePlayer.Resources = ActivePlayer.Resources.UseResource(health: ActivePlayer.Resources.Health);
            }

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
