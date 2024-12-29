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
    internal class Wraith: BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "Wraith",
            MaxHealth = 9,
            MoveCost = 2,
            NPCID = NPCID.Wraith,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.EXPERT, CardSubtype.FOREST, CardSubtype.FIGHTER],
            Modifiers = () => [
                new EvasiveModifier(),
            ],
            Attacks = [
                new() {
                    Name = "Haunt",
                    Damage = 4,
                    Cost = 2,
                }
            ]
        };
    }
}
