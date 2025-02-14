using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState.Modifiers
{
    internal class RegenerationModifier(int amount, List<GameEvent> removeOn = null) : ICardModifier
    {
        public int Amount => amount;

        public Asset<Texture2D> Texture { get; set; }
		public Card SourceCard { get; set; }

        public bool ShouldRemove(GameEventInfo eventInfo) {
			if(eventInfo.Event == GameEvent.END_TURN && eventInfo.IsMyTurn)
			{
				eventInfo.Zone.PlacedCard?.Heal(amount);
			}
            return !eventInfo.IsMyTurn && (removeOn?.Contains(eventInfo.Event) ?? false);
        }
    }
}
