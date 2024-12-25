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
    internal class BlueSlime : ModSystem, ICardTemplate
    {
        public Card CreateCard() => new ()
        {
            Name = "BlueSlime",
            MaxHealth = 6,
            MoveCost = 2,
            NPCID = NPCID.BlueSlime,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.FOREST, CardSubtype.SLIME, CardSubtype.FIGHTER],
            Attacks = [
                new() {
                    Name = "Slimed",
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }
}
