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
    internal class SnowFlinx : BaseCardTemplate, ICardTemplate
    {
        internal class AttackCostModifier : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                attack.Cost += 1;
            }

            public bool ShouldRemove(GameEventInfo eventInfo) => 
                eventInfo.Event == GameEvent.AFTER_ATTACK; 
        }

        public override Card CreateCard() => new ()
        {
            Name = "SnowFlinx",
            MaxHealth = 6,
            MoveCost = 2,
            NPCID = NPCID.SnowFlinx,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.SNOW, CardSubtype.SCOUT],
            Attacks = [
                new() {
                    Name = "Hex",
                    Damage = 1,
                    Cost = 1,
                    TargetModifiers = t=>[new AttackCostModifier()],
                }
            ]
        };
    }
}
