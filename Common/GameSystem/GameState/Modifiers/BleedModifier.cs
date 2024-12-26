using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState.Modifiers
{
    internal class BleedModifier(int amount, List<GameEvent> removeOn = null) : ICardModifier
    {
        public int Amount => 1;
        public ModifierType Category => ModifierType.BLEEDING;

        public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
        {
            // no-op
            attack.Damage += amount;
        }

        public bool ShouldRemove(GameEventInfo eventInfo) =>
            eventInfo.IsMyTurn && (removeOn?.Contains(eventInfo.Event) ?? false);
    }
}
