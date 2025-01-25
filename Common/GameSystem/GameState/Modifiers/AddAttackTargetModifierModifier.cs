using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState.Modifiers
{
	// Meta-modifier that adds a modifier to the attack of the
	// card that this modifies
	internal class AddAttackTargetModifierModifier(ICardModifier targetModifier, List<GameEvent> removeOn = null) : ICardModifier
	{
		public Card SourceCard { get; set; }

        public Asset<Texture2D> Texture { get; set; }

		public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
		{
			// higher order functions yay
			var localAttack = attack;
			attack.TargetModifiers = (zone) => [targetModifier, .. localAttack.TargetModifiers?.Invoke(zone) ?? []];
		}
		public bool ShouldRemove(GameEventInfo eventInfo) {
			return removeOn?.Contains(eventInfo.Event) ?? false;
		}
	}
}
