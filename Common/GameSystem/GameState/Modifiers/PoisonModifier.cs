using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.Drawing.Animations;

namespace TerraTCG.Common.GameSystem.GameState.Modifiers
{
	internal class PoisonModifier : ICardModifier
	{
        public ModifierType Category { get => ModifierType.POISON; }

		// Ensure that 'ShouldRemove' calls are idempotent
		private bool didApplyThisTurn;

		private bool ShouldDeduplicate(GameEventInfo eventInfo)
		{
			// TODO this is a bit funky, prevent multiple stacks of poison
			var firstPoisonInstance = eventInfo.Zone.PlacedCard?.CardModifiers
				.Where(c => c.Category == ModifierType.POISON).FirstOrDefault();
			return firstPoisonInstance != this; 
		}

        public bool ShouldRemove(GameEventInfo eventInfo) {
			if(ShouldDeduplicate(eventInfo))
			{
				return true;
			}

			if(eventInfo.Event == GameEvent.START_TURN)
			{
				didApplyThisTurn = false;
			} else if (eventInfo.Event == GameEvent.END_TURN && !didApplyThisTurn 
				&& eventInfo.Zone.PlacedCard is PlacedCard card && eventInfo.Zone.Owner == eventInfo.TurnPlayer)
			{
				didApplyThisTurn = true;
				card.CurrentHealth -= 1;
				eventInfo.Zone.QueueAnimation(
					new ApplyModifierAnimation(card, TextureCache.Instance.ModifierIconTextures[ModifierType.POISON])
				);
			}
            return false;
        }
	}
}
