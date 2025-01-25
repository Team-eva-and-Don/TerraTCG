using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class DoctorBones : BaseCardTemplate, ICardTemplate
    {
        private class ControlsCritterModifier : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                var critterCount = sourceZone.Siblings
                    .Where(z => z.PlacedCard?.Template?.SubTypes?.Contains(CardSubtype.CRITTER) ?? false)
                    .Count();
				attack.Damage += critterCount;
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "DoctorBones",
            MaxHealth = 9,
            MoveCost = 2,
            NPCID = NPCID.DoctorBones,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.JUNGLE, CardSubtype.FIGHTER],
            Modifiers = () => [
                new ControlsCritterModifier()
            ],
            Attacks = [
                new() {
                    Name = "Research",
                    Damage = 3,
                    Cost = 3,
                }
            ]
        };
    }
}
