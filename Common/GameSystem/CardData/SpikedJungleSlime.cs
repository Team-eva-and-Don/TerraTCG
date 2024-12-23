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
    internal class SpikedJungleSlime : ModSystem, ICardTemplate
    {
        public Card CreateCard() => new ()
        {
            Name = "SpikedJungleSlime",
            MaxHealth = 9,
            MoveCost = 3,
            CardType = CardType.CREATURE,
            NPCID = NPCID.SpikedJungleSlime,
            SubTypes = [CardSubtype.JUNGLE, CardSubtype.DEFENDER],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 3,
                }
            ],
            Modifiers = [
                new SpikedModifier(2)
            ]
        };
    }
}
