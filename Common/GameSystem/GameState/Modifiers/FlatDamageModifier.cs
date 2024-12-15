using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState.Modifiers
{
    internal class FlatDamageModifier(int modifier, List<GameEvent> removeOn = null) : ICardModifier
    {
        public string Texture { get; set; } = "";
        public string Description { get; set; } = "";

        public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone)
        {
            attack.Damage += modifier;
        }

        public bool ShouldRemove(GameEvent gameEvent) {
            return removeOn?.Contains(gameEvent) ?? false;
        }

    }
}
