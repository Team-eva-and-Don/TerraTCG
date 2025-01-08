using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class Salamander : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "Salamander",
            MaxHealth = 8,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Salamander,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.SCOUT],
            Role = ZoneRole.OFFENSE,
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ],
            Skills = [
                new() {
                    Name = "Skill: Slippery Scales",
                    Cost = 0,
                    DoSkill = (GamePlayer player, Zone zone, Zone targetZone) => {
						foreach (var sibling in zone.Siblings) {
							if(sibling.PlacedCard?.Template is Card card && (card.Name == "Crawdad" || card.Name == "GiantShelly")) {
								sibling.PlacedCard.AddModifiers(
									[new AttackCostReductionModifier(1, [GameEvent.END_TURN])]);
							}
						}
                    }
                }
            ]
        };
    }
}
