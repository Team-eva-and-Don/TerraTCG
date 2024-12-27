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
    internal class Mimic : ModSystem, ICardTemplate
    {
        private class EquipCostModifier : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                var itemCount = sourceZone.Owner.Game.Turns
                    .Where(t => t.ActivePlayer == sourceZone.Owner)
                    .Select(t => t.UsedItemCount)
                    .Sum();
                attack.Cost = Math.Max(1, attack.Cost - itemCount);
            }
        }

        public Card CreateCard() => new ()
        {
            Name = "Mimic",
            MaxHealth = 8,
            MoveCost = 2,
            NPCID = NPCID.Mimic,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.DEFENDER],
            DrawZoneNPC = CardOverlayRenderer.Instance.DrawMimicNPC,
            Modifiers = [
                new EquipCostModifier(),
                new ReduceDamageModifier(1),
            ],
            Attacks = [
                new() {
                    Name = "Lure",
                    Damage = 5,
                    Cost = 6,
                }
            ]
        };
    }
}
