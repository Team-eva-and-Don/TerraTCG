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
    internal class Crab : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "Crab",
            MaxHealth = 7,
            MoveCost = 2,
            NPCID = NPCID.Crab,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.OCEAN, CardSubtype.FIGHTER],
            Modifiers = [
                new RelentlessModifier(),
            ],
            Attacks = [
                new() {
                    Name = "Pinch",
                    Damage = 1,
                    Cost = 1,
                }
            ]
        };
    }
}
