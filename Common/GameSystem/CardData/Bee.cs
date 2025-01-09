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
    internal class Bee : BaseCardTemplate, ICardTemplate
    {
		private class ApplyPoisonThisTurnModifier : ICardModifier
		{
			public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				// higher order functions yay
				var localAttack = attack;
				attack.TargetModifiers = (zone) => [new PoisonModifier(), .. localAttack.TargetModifiers?.Invoke(zone) ?? []];
			}
			public bool ShouldRemove(GameEventInfo eventInfo) {
				return eventInfo.Event == GameEvent.END_TURN;
			}
		}

        public override Card CreateCard() => new ()
        {
            Name = "Bee",
            MaxHealth = 5,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Bee,
            SubTypes = [CardSubtype.JUNGLE, CardSubtype.CRITTER],
            Role = ZoneRole.DEFENSE,
            Attacks = [
                new() {
                    Damage = 1,
                    Cost = 1,
                }
            ],
            Skills = [
                new() {
                    Name = "Skill: Toxic Honey",
                    Cost = 1,
                    SkillType = ActionType.TARGET_ALLY,
                    DoSkill = (GamePlayer player, Zone zone, Zone targetZone) => {
						targetZone.PlacedCard.AddModifiers([new AddAttackTargetModifierModifier(new PoisonModifier(), [GameEvent.END_TURN])]);
					}
                }
            ]
        };
    }
}
