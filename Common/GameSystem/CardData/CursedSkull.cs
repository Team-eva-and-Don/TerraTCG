using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class CursedSkull : BaseCardTemplate, ICardTemplate
    {
        // TODO This is a bit of an odd implementation, modifier to make the skill
        // unusably expensive if an item has not been used this turn
        internal class SkillCostModifier : ICardModifier
        {
            public void ModifySkill(ref Skill skill, Zone sourceZone, Zone destZone) 
            {
                skill.Cost = 999;
            }
            public bool ShouldRemove(GameEventInfo eventInfo) => 
                eventInfo.IsMyTurn && eventInfo.Event == GameEvent.END_TURN; 
        }

        public override Card CreateCard() => new ()
        {
            Name = "CursedSkull",
            MaxHealth = 6,
            MoveCost = 2,
            NPCID = NPCID.CursedSkull,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.CASTER],
            Modifiers = () => [
                new EvasiveModifier(),
            ],
            Attacks = [
                new() {
                    Name = "Curse",
                    Damage = 2,
                    Cost = 2,
                    TargetModifiers = t=>[new SkillCostModifier()],
                }
            ]
        };
    }
}
