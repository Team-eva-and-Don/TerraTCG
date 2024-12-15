using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal delegate void DoAttack(Attack attack, Zone sourceZone, Zone targetZone);
    internal struct Attack()
    {
        internal string Name { get; set; }
        internal string Description { get; set; }
        internal int Damage { get; set; }
        internal int Cost { get; set; }

        internal DoAttack DoAttack { get; set; } = DefaultAttack;


        internal static void DefaultAttack(Attack attack, Zone sourceZone, Zone targetZone)
        {
            targetZone.PlacedCard.CurrentHealth -= attack.Damage;
        }

        internal Attack Copy()
        {
            return new Attack() 
            {
                Name = Name,
                Description = Description,
                Damage = Damage,
                Cost = Cost,
                DoAttack = DoAttack
            };
        }
    }
}
