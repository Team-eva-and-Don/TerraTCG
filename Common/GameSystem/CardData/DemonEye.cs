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
    internal class DemonEye : ModSystem, ICardTemplate
    {
        internal static ICardTemplate Instance => ModContent.GetInstance<DemonEye>();
        public Card CreateCard() => new ()
        {
            Name = "DemonEye",
            MaxHealth = 6,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.DemonEye,
            SubTypes = [CardSubtype.FOREST, CardSubtype.EYE],
            Attacks = [
                new() {
                    Damage = 2,
                    Cost = 2,
                    Description = "H"
                }
            ],
            Modifiers = [
                new EvasiveModifier(),
            ]
        };
    }
}
