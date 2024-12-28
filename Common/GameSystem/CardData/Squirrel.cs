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
    internal class Squirrel : BaseCardTemplate, ICardTemplate
    {
        private void ForestCheer(GamePlayer player, Zone zone, Zone targetZone)
        {
            targetZone.PlacedCard.AddModifiers([new FlatDamageModifier(1, [GameEvent.AFTER_ATTACK, GameEvent.END_TURN])]);
        }

        public override Card CreateCard() => new ()
        {
            Name = "Squirrel",
            MaxHealth = 5,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Squirrel,
            SubTypes = [CardSubtype.FOREST, CardSubtype.CRITTER],
            Role = ZoneRole.DEFENSE,
            Attacks = [
                new() {
                    Name = "Acorn Toss",
                    Damage = 1,
                    Cost = 1,
                }
            ],
            Skills = [
                new() {
                    Name = "Skill: Forest Cheer",
                    Cost = 1,
                    SkillType = ActionType.TARGET_ALLY,
                    DoSkill = ForestCheer,
                }
            ]
        };
    }
}
