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
    internal class Shark : ModSystem, ICardTemplate
    {
        public Card CreateCard() => new ()
        {
            Name = "Shark",
            MaxHealth = 9,
            MoveCost = 3,
            NPCID = NPCID.Shark,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.OCEAN, CardSubtype.FIGHTER],
            Modifiers = [
                new RelentlessModifier(),
            ],
            Attacks = [
                new() {
                    Name = "Chomp",
                    Damage = 4,
                    Cost = 3,
                }
            ]
        };
    }
}
