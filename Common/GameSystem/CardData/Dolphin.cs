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
    internal class Dolphin : ModSystem, ICardTemplate
    {
        public Card CreateCard() => new ()
        {
            Name = "Dolphin",
            MaxHealth = 5,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Dolphin,
            SubTypes = [CardSubtype.OCEAN, CardSubtype.CRITTER],
            Role = ZoneRole.DEFENSE,
            Attacks = [
                new() {
                    Damage = 1,
                    Cost = 1,
                }
            ],
            Skills = [
                new() {
                    Name = "Skill: Ocean Wish",
                    Cost = 0,
                    SkillType = ActionType.TARGET_ALLY,
                    DoSkill = (GamePlayer player, Zone zone, Zone targetZone) => {
                        targetZone.PlacedCard.AddModifiers(
                            [new AttackCostReductionModifier(1, [GameEvent.END_TURN])]
                        );
                    }
                }
            ]
        };
    }
}
