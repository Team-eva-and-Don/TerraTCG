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
    internal class ViciousBunny : BaseCardTemplate, ICardTemplate
    {
        private void BloodyCheer(GamePlayer player, Zone zone, Zone targetZone)
        {
            targetZone.PlacedCard.AddModifiers([new FlatDamageModifier(1, [GameEvent.END_TURN])]);
            targetZone.PlacedCard.AddModifiers([new BleedModifier(1)]);
        }

        public override Card CreateCard() => new ()
        {
            Name = "ViciousBunny",
            MaxHealth = 5,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.CrimsonBunny,
            SubTypes = [CardSubtype.BLOOD_MOON, CardSubtype.CRITTER],
            Role = ZoneRole.DEFENSE,
            Attacks = [
                new() {
                    Name = "Bloody Kick",
                    Damage = 1,
                    Cost = 1,
                }
            ],
            Skills = [
                new() {
                    Name = "Skill: Forest Cheer",
                    Cost = 0,
                    SkillType = ActionType.TARGET_ALLY,
                    DoSkill = BloodyCheer,
                }
            ]
        };
    }
}
