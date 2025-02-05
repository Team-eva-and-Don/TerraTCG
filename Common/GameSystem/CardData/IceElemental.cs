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
    internal class IceElemental : BaseCardTemplate, ICardTemplate
    {
        internal class AttackCostModifier : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
				// Set this card's attack to the attack cost of the creature it's attacking
				var targetZoneAttackCost = destZone?.HasPlacedCard() ?? false ?
					destZone.PlacedCard.GetAttackWithModifiers(destZone, null).Cost : 0;

				attack.Damage = targetZoneAttackCost;
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "IceElemental",
            MaxHealth = 6,
            MoveCost = 2,
            NPCID = NPCID.IceElemental,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.SNOW, CardSubtype.CASTER],
            DrawZoneNPC = CardOverlayRenderer.Instance.DrawMimicNPC,
            Modifiers = () => [
                new AttackCostModifier(),
				new FreezingModifier(1),
            ],
            Attacks = [
                new() {
                    Damage = -1,
                    Cost = 3,
                }
            ]
        };
    }
}
