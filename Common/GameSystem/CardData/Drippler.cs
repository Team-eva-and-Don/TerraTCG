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
    internal class Drippler : ModSystem, ICardTemplate
    {
        public Card CreateCard() => new ()
        {
            Name = "Drippler",
            MaxHealth = 5,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Drippler,
            SubTypes = [CardSubtype.BLOOD_MOON, CardSubtype.SCOUT],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                    SelfDamage = 1,
                }
            ],
            Modifiers = [
                new ZealousModifier(),
            ]
        };
    }
}
