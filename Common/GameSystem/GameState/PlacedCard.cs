using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal class PlacedCard(Card template)
    {
        internal Card Template { get; set; } = template;

        internal int CurrentHealth { get; set; } = template.MaxHealth;

        internal List<ICardModifier> CardModifiers { get; set; } = [];

        public Attack GetAttackWithModifiers(Zone startZone, Zone endZone)
        {
            var attack = Template.Attacks[0].Copy();
            foreach (var modifier in CardModifiers)
            {
                modifier.ModifyAttack(ref attack, startZone, endZone);
            }
            return attack;
        }

    }
}
