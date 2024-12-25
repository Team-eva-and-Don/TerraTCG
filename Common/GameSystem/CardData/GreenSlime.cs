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
    internal class GreenSlime : ModSystem, ICardTemplate
    {
        public Card CreateCard() => new ()
        {
            Name = "GreenSlime",
            MaxHealth = 5,
            MoveCost = 2,
            NPCID = NPCID.BlueSlime,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.FOREST, CardSubtype.SLIME, CardSubtype.SCOUT],
            Attacks = [
                new() {
                    Name = "Slimed",
                    Damage = 2,
                    Cost = 1,
                }
            ]
        };
    }
}
