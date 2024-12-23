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
    internal class Goldfish: ModSystem, ICardTemplate
    {
        private void PondHealing(GamePlayer player, Zone zone, Zone targetZone)
        {
            targetZone.PlacedCard.Heal(1);
        }

        public Card CreateCard() => new ()
        {
            Name = "Goldfish",
            MaxHealth = 6,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.GoldfishWalker,
            SubTypes = [CardSubtype.FOREST, CardSubtype.CRITTER],
            Role = ZoneRole.DEFENSE,
            Attacks = [
                new() {
                    Name = "Fin Slap",
                    Damage = 1,
                    Cost = 1,
                }
            ],
            Skills = [
                new() {
                    Name = "Skill: Pond Healing",
                    Cost = 1,
                    Role = ZoneRole.DEFENSE,
                    SkillType = ActionType.TARGET_ALLY,
                    DoSkill = PondHealing,
                }
            ]
        };
    }
}
