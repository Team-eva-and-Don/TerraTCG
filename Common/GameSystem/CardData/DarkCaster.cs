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
    internal class DarkCaster : BaseCardTemplate, ICardTemplate
    {
        // TODO This is a bit of an odd implementation, modifier to make the skill
        // unusably expensive if an item has not been used this turn
        internal class AttackCostModifier : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                attack.Cost += 2;
            }
            public bool ShouldRemove(GameEventInfo eventInfo) => 
                eventInfo.IsMyTurn && eventInfo.Event == GameEvent.END_TURN; 
        }

        public override Card CreateCard() => new ()
        {
            Name = "DarkCaster",
            MaxHealth = 6,
            MoveCost = 2,
            NPCID = NPCID.DarkCaster,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.CASTER],
            Attacks = [
                new() {
                    Name = "Hex",
                    Damage = 2,
                    Cost = 2,
                    TargetModifiers = t=>[new AttackCostModifier()],
                }
            ]
        };
    }
}
