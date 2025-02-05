using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class IceBat : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "IceBat",
            MaxHealth = 7,
            NPCID = NPCID.IceBat,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.SNOW, CardSubtype.SCOUT],
            Modifiers = () => [
                new FreezingModifier(1),
                new EvasiveModifier(),
            ],
            Attacks = [
                new() {
                    Damage = 2,
                    Cost = 3,
                }
            ]
        };
    }
}
