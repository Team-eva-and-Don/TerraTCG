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
    internal class Penguin : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "Penguin",
            MaxHealth = 5,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.PenguinBlack,
            SubTypes = [CardSubtype.SNOW, CardSubtype.CRITTER],
            Role = ZoneRole.DEFENSE,
            Attacks = [
                new() {
                    Damage = 1,
                    Cost = 1,
                }
            ],
            Skills = [
                new() {
                    Cost = 1,
                    SkillType = ActionType.TARGET_ALLY,
                    DoSkill = (GamePlayer player, Zone zone, Zone targetZone) => {
						targetZone.PlacedCard.AddModifiers([new FreezingModifier(1, [GameEvent.END_TURN])]);
					}
                }
            ]
        };
    }
}
