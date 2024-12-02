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
    internal class Bunny : ModSystem, ICardTemplate
    {
        internal static ICardTemplate Instance => ModContent.GetInstance<Bunny>();
        public Card CreateCard() => new ()
        {
            Name = "Bunny",
            MaxHealth = 40,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Bunny,
            SubTypes = [CardSubtype.FOREST, CardSubtype.CRITTER],
            Attacks = [
                new() {
                    Damage = 30,
                    Cost = 2,
                }
            ]
        };
    }
}
