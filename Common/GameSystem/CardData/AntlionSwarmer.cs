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
    internal class AntlionSwarmer : ModSystem, ICardTemplate
    {
        public Card CreateCard() => new ()
        {
            Name = "AntlionSwarmer",
            MaxHealth = 5,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.FlyingAntlion,
            SubTypes = [CardSubtype.DESERT, CardSubtype.SCOUT],
            Attacks = [
                new() {
                    Damage = 2,
                    Cost = 2,
                }
            ],
            Modifiers = [
                new ZealousModifier(),
                new EvasiveModifier(),
            ]
        };
    }
}
