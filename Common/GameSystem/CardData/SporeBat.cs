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
    internal class SporeBat : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "SporeBat",
            MaxHealth = 7,
            MoveCost = 2,
            NPCID = NPCID.SporeBat,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.MUSHROOM, CardSubtype.SCOUT],
            Modifiers = [
                new EvasiveModifier(),
                new LifestealModifier(1),
            ],
            Attacks = [
                new() {
                    Name = "Spore Wing",
                    Damage = 1,
                    Cost = 1,
                }
            ]
        };
    }
}
