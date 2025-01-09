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
    internal class GiantShelly : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "GiantShelly",
            MaxHealth = 8,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.GiantShelly,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.DEFENDER],
            Role = ZoneRole.OFFENSE,
            Attacks = [
                new() {
                    Damage = 2,
                    Cost = 2,
                }
            ],
            Skills = [
                new() {
                    Name = "Skill: Hard Shell",
                    Cost = 1,
                    DoSkill = (GamePlayer player, Zone zone, Zone targetZone) => {
						foreach (var sibling in zone.Siblings) {
							if(sibling.PlacedCard?.Template is Card card && (card.Name == "Crawdad" || card.Name == "Salamander")) {
								sibling.PlacedCard.AddModifiers(
									[new ReduceDamageModifier(1, [GameEvent.END_TURN])]);
							}
						}
                    }
                }
            ]
        };
    }
}
