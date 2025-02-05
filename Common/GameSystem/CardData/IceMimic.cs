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
    internal class IceMimic : BaseCardTemplate, ICardTemplate
    {
        private class AttackCostModifier : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
				// Reduce this attack's cost by the sum of all opponent attack costs
				var targetZoneAttackCost = destZone?.HasPlacedCard() ?? false ?
					destZone.PlacedCard.GetAttackWithModifiers(destZone, null).Cost : 0;

                attack.Cost = Math.Max(1, attack.Cost - targetZoneAttackCost);
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "IceMimic",
            MaxHealth = 8,
            MoveCost = 2,
            NPCID = NPCID.IceMimic,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.SNOW, CardSubtype.DEFENDER],
            DrawZoneNPC = CardOverlayRenderer.Instance.DrawMimicNPC,
            Modifiers = () => [
                new AttackCostModifier(),
                new FreezingModifier(1),
            ],
            Attacks = [
                new() {
                    Damage = 5,
                    Cost = 8,
                }
            ]
        };
    }
}
