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
    internal class GlowingSnail : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "GlowingSnail",
            MaxHealth = 6,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.GlowingSnail,
            SubTypes = [CardSubtype.MUSHROOM, CardSubtype.CRITTER],
            Role = ZoneRole.DEFENSE,
            Attacks = [
                new() {
                    Damage = 1,
                    Cost = 1,
                }
            ],
            Skills = [
                new() {
                    Name = "Skill: Spore Storm",
                    Cost = 1,
                    DoSkill = (GamePlayer player, Zone zone, Zone targetZone) => {
                        player.Field.CardModifiers.Add(new LifestealModifier(1, [GameEvent.END_TURN]));
                    }
                }
            ]
        };
    }
}
