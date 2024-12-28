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
    internal class PosessedArmor: BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "PosessedArmor",
            MaxHealth = 9,
            MoveCost = 2,
            NPCID = NPCID.PossessedArmor,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.EXPERT, CardSubtype.FOREST, CardSubtype.FIGHTER],
            Modifiers = [
                new ReduceDamageModifier(1),
            ],
            Attacks = [
                new() {
                    Name = "Cursed Steel",
                    Damage = 4,
                    Cost = 2,
                }
            ]
        };
    }
}
