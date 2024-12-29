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
    internal class Jellyfish : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "Jellyfish",
            MaxHealth = 7,
            MoveCost = 2,
            NPCID = NPCID.PinkJellyfish,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.OCEAN, CardSubtype.FIGHTER],
            Modifiers = () => [
                new RelentlessModifier(),
            ],
            Attacks = [
                new() {
                    Name = "Sting",
                    Damage = 2,
                    Cost = 2,
                }
            ]
        };
    }
}
