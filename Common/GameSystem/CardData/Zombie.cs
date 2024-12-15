using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class Zombie : ModSystem, ICardTemplate
    {
        internal static ICardTemplate Instance => ModContent.GetInstance<Zombie>();
        public Card CreateCard() => new ()
        {
            Name = "Zombie",
            MaxHealth = 7,
            MoveCost = 2,
            NPCID = NPCID.Zombie,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.FOREST, CardSubtype.UNDEAD],
            Attacks = [
                new() {
                    Name = "Diseased Bite",
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }
}
