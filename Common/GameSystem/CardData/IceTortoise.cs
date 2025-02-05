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
    internal class IceTortoise : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "IceTortoise",
            MaxHealth = 9,
            NPCID = NPCID.IceTortoise,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.EXPERT, CardSubtype.SNOW, CardSubtype.DEFENDER],
            FieldModifiers = () => [
                new FreezingModifier(1, [GameEvent.START_TURN], removeOnMyTurn: true),
            ],
            Attacks = [
                new() {
                    Damage = 4,
                    Cost = 5,
                }
            ]
        };
    }
}
