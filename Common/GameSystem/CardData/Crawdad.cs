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
    internal class Crawdad : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "Crawdad",
            MaxHealth = 7,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Crawdad,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.FIGHTER],
            Role = ZoneRole.OFFENSE,
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ],
            Skills = [
                new() {
                    Name = "Skill: Sharp Claws",
                    Cost = 1,
                    DoSkill = (GamePlayer player, Zone zone, Zone targetZone) => {
						foreach (var sibling in zone.Siblings) {
							if(sibling.PlacedCard?.Template is Card card && (card.Name == "Salamander" || card.Name == "GiantShelly")) {
								sibling.PlacedCard.AddModifiers(
									[new FlatDamageModifier(1, [GameEvent.END_TURN])]);
							}
						}
                    }
                }
            ]
        };
    }
}
