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
    internal class Bat : ModSystem, ICardTemplate
    {
        public Card CreateCard() => new ()
        {
            Name = "Bat",
            MaxHealth = 5,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.CaveBat,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.BAT],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ],
            Modifiers = [
                new EvasiveModifier(),
            ]
        };
    }
}
