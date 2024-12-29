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
    internal class UndeadMiner : BaseCardTemplate, ICardTemplate
    {
        internal class EquipCostModifier : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                var equipCount = sourceZone.PlacedCard?.CardModifiers
                    .Where(m => m.Source == CardSubtype.EQUIPMENT).Count() ?? 0;
                attack.Cost = Math.Max(1, attack.Cost - equipCount);
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "UndeadMiner",
            MaxHealth = 8,
            MoveCost = 2,
            NPCID = NPCID.UndeadMiner,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.FIGHTER],
            Modifiers = () => [
                new EquipCostModifier()
            ],
            Attacks = [
                new() {
                    Name = "Pickaxe Swing",
                    Damage = 4,
                    Cost = 4,
                }
            ]
        };
    }
}
