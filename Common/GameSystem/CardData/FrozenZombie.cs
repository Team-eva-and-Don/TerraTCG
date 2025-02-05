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
    internal class FrozenZombie : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "FrozenZombie",
            MaxHealth = 8,
            NPCID = NPCID.ZombieEskimo,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.SNOW, CardSubtype.DEFENDER],
            Modifiers = () => [
                new FreezingModifier(1),
            ],
            Attacks = [
                new() {
                    Damage = 2,
                    Cost = 2,
                }
            ]
        };
    }
}
