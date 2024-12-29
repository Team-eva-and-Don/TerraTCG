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
    internal class Bat : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "Bat",
            MaxHealth = 5,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.CaveBat,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.SCOUT],
            Attacks = [
                new() {
                    Damage = 2,
                    Cost = 2,
                }
            ],
            Modifiers = () => [
                new EvasiveModifier(),
            ]
        };
    }
}
