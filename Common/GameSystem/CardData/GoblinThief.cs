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
    internal class GoblinThief : BaseCardTemplate, ICardTemplate
    {
        // TODO This is a bit of an odd implementation, modifier to make the skill
        // unusably expensive if an item has not been used this turn
        internal class SkillCostModifier : ICardModifier
        {
            public void ModifySkill(ref Skill skill, Zone sourceZone, Zone destZone) 
            {
                var itemCount = sourceZone.Owner.Game.CurrentTurn.UsedItemCount;
                skill.Cost = itemCount == 0 ? 999 : 2;
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "GoblinThief",
            MaxHealth = 5,
            MoveCost = 1,
            NPCID = NPCID.GoblinThief,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.GOBLIN_ARMY, CardSubtype.SCOUT],
            Role = ZoneRole.DEFENSE,
            Skills = [
                new() {
                    Cost = 2,
                    DoSkill = (GamePlayer player, Zone cardZone, Zone endZone) => { 
                        if(!player.Deck.Empty()) {
                            player.Hand.Add(player.Deck.Draw());
                        }
                    },
                }
            ],
            Modifiers = [
                new SkillCostModifier(),
            ],
            Attacks = [
                new() {
                    Name = "Shank",
                    Damage = 2,
                    Cost = 2,
                }
            ]
        };
    }
}
