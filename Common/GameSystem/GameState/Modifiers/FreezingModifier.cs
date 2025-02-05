using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState.Modifiers
{
	internal class IncreaseNextAttackCostModifier(int amount) : ICardModifier
	{
		// ShouldRemove(event=AFTER_ATTACK) is called immediately after the modifier is applied,
		// Need to wait for the _next_ attack after that to remove
		internal bool hasAttackedOnce; 

        public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
        {
			attack.Cost += amount;
        }

		public bool ShouldRemove(GameEventInfo eventInfo)
		{
			if (eventInfo.Event == GameEvent.AFTER_ATTACK)
			{
				bool toReturn = hasAttackedOnce;
				hasAttackedOnce = true;
				return toReturn;
			}
			return false;
		}
	}


    internal class FreezingModifier(int amount, List<GameEvent> removeOn = null, bool removeOnMyTurn = false) : ICardModifier
    {
        public int Amount => amount;

        public Asset<Texture2D> Texture { get; set; }
		public Card SourceCard { get; set; }
        public ModifierType Category => ModifierType.FREEZING;

        public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
        {
			var localAttack = attack;
			attack.SourceModifiers = (zone) => [new IncreaseNextAttackCostModifier(amount), .. localAttack.SourceModifiers?.Invoke(zone) ?? []];
        }

        public bool ShouldRemove(GameEventInfo eventInfo) {
            return (removeOnMyTurn || !eventInfo.IsMyTurn) && (removeOn?.Contains(eventInfo.Event) ?? false);
        }
    }
}
