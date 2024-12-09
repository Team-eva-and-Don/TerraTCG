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
    internal class Bat : ModSystem, ICardTemplate
    {
        internal static ICardTemplate Instance => ModContent.GetInstance<Bat>();
        public Card CreateCard() => new ()
        {
            Name = "Bat",
            MaxHealth = 50,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.CaveBat,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.BAT],
            Attacks = [
                new() {
                    Damage = 30,
                    Cost = 2,
                }
            ],
            Modifiers = [
                new() {
                    Description = "Evasive",
                }
            ]
        };
    }
}
