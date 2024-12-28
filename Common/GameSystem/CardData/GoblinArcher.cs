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
    internal class GoblinArcher : BaseCardTemplate, ICardTemplate
    {

        public override Card CreateCard() => new ()
        {
            Name = "GoblinArcher",
            MaxHealth = 7,
            MoveCost = 1,
            NPCID = NPCID.GoblinArcher,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.GOBLIN_ARMY, CardSubtype.SCOUT],
            Modifiers = [
                new EvasiveModifier(),
                new GoblinScout.DamageModifier(),
            ],
            Attacks = [
                new() {
                    Name = "GoblinArcher",
                    Damage = 1,
                    Cost = 1,
                }
            ]
        };
    }
}
