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
    internal class MushroomZombie : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "MushroomZombie",
            MaxHealth = 8,
            MoveCost = 2,
            NPCID = NPCID.ZombieMushroomHat,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.MUSHROOM, CardSubtype.FIGHTER],
            Modifiers = [
                new LifestealModifier(1),
            ],
            Attacks = [
                new() {
                    Name = "Spore Bite",
                    Damage = 2,
                    Cost = 2,
                }
            ]
        };
    }
}
