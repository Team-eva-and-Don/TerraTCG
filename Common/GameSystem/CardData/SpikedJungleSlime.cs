using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
    internal class SpikedJungleSlime : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "SpikedJungleSlime",
            MaxHealth = 8,
            MoveCost = 3,
            CardType = CardType.CREATURE,
            NPCID = NPCID.SpikedJungleSlime,
            SubTypes = [CardSubtype.JUNGLE, CardSubtype.SLIME, CardSubtype.DEFENDER],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 3,
                }
            ],
            Modifiers = () => [
                new SpikedModifier(2)
            ]
        };
    }
}
