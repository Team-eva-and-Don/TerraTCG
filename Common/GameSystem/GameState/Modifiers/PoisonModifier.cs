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
		public int Amount => 1;

		// Ensure that 'ShouldRemove' calls are idempotent
		private bool didApplyThisTurn;

        public bool ShouldRemove(GameEventInfo eventInfo) {
			if(eventInfo.Event == GameEvent.START_TURN)
			{
				didApplyThisTurn = false;
			} else if (eventInfo.Event == GameEvent.END_TURN && !didApplyThisTurn 
				&& eventInfo.Zone.PlacedCard is PlacedCard card && eventInfo.Zone.Owner == eventInfo.TurnPlayer)
			{
				didApplyThisTurn = true;
				card.CurrentHealth -= 1;
				// Only queue one poison animation regardless of stack size
				if(this == card.CardModifiers.Where(m=>m.Category == ModifierType.POISON).First())
				{
					eventInfo.Zone.QueueAnimation(
						new ApplyModifierAnimation(card, TextureCache.Instance.ModifierIconTextures[ModifierType.POISON])
					);
				}
			}
            return false;
        }
	}
}
