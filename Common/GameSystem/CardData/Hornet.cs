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
    internal class Hornet : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "Hornet",
            MaxHealth = 6,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Hornet,
            SubTypes = [CardSubtype.JUNGLE, CardSubtype.SCOUT],
            Attacks = [
                new() {
                    Damage = 1,
                    Cost = 2,
                    TargetModifiers = z =>[new PoisonModifier()]
                }
            ],
        };
    }
}
