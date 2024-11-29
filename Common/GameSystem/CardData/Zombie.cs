using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class Zombie : ModSystem, ICardTemplate
    {
        internal static Zombie Instance => ModContent.GetInstance<Zombie>();
        public Card CreateCard() => new ()
        {
            Name = "Zombie",
            MaxHealth = 70,
            MoveCost = 3,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.FOREST, CardSubtype.UNDEAD],
            Attacks = [
                new() {
                    Damage = 30,
                }
            ]
        };
    }
}
