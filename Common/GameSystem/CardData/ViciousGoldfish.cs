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
    internal class ViciousGoldfish : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "ViciousGoldfish",
            MaxHealth = 5,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.CrimsonGoldfish,
            SubTypes = [CardSubtype.BLOOD_MOON, CardSubtype.SCOUT],
            Attacks = [
                new() {
                    Damage = 2,
                    Cost = 1,
                }
            ],
            Modifiers = [
                new ZealousModifier(),
                new FlatDamageModifier(1, removeOn: [GameEvent.END_TURN]),
            ]
        };
    }
}
