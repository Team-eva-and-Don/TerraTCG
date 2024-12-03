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
    internal class DemonEye : ModSystem, ICardTemplate
    {
        internal static ICardTemplate Instance => ModContent.GetInstance<DemonEye>();
        public Card CreateCard() => new ()
        {
            Name = "DemonEye",
            MaxHealth = 60,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.DemonEye,
            SubTypes = [CardSubtype.FOREST, CardSubtype.EYE],
            Attacks = [
                new() {
                    Name = "Harsh Gaze",
                    Damage = 20,
                    Cost = 2,
                }
            ],
            Modifiers = [
                new() {
                    Name = "Evasive",
                    Description = "May attack any enemy"
                }
            ]
        };
    }
}
