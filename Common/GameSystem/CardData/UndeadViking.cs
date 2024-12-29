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
    internal class UndeadViking : BaseCardTemplate, ICardTemplate
    {
        private class DamageModifier : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                var isEquipped = sourceZone.PlacedCard?.CardModifiers
                    .Where(m => m.Source == CardSubtype.EQUIPMENT).Any() ?? false;
                if(isEquipped)
                {
                    attack.Damage += 1;
                }
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "UndeadViking",
            MaxHealth = 8,
            MoveCost = 2,
            NPCID = NPCID.UndeadViking,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.FIGHTER],
            Modifiers = () => [
                new DamageModifier()
            ],
            Attacks = [
                new() {
                    Name = "Freezing Axe",
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }
}
