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
    internal class SporeSkeleton : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "SporeSkeleton",
            MaxHealth = 9,
            MoveCost = 2,
            NPCID = NPCID.SporeSkeleton,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.MUSHROOM, CardSubtype.FIGHTER],
            Modifiers = () => [
                new LifestealModifier(1),
            ],
            Attacks = [
                new() {
                    Name = "Spore Bite",
                    Damage = 3,
                    Cost = 3,
                }
            ]
        };
    }
}
