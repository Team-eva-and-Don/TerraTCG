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
    internal class GoblinWarlock : BaseCardTemplate, ICardTemplate
    {
        internal class DamageModifier : ICardModifier
        {
            private int startingItemCount;

            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                var currentItemCount = sourceZone.Owner.Game.Turns
                    .Where(t => t.ActivePlayer == sourceZone.Owner)
                    .Select(t => t.UsedItemCount)
                    .Sum();
                attack.Damage += currentItemCount;
            }
            public void ModifyCardEntrance(Zone sourceZone) 
            {
                startingItemCount = sourceZone.Owner.Game.Turns
                    .Where(t => t.ActivePlayer == sourceZone.Owner)
                    .Select(t => t.UsedItemCount)
                    .Sum();
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "GoblinWarlock",
            MaxHealth = 10,
            MoveCost = 1,
            NPCID = NPCID.GoblinSummoner,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.EXPERT, CardSubtype.GOBLIN_ARMY, CardSubtype.SCOUT],
            Modifiers = [
                new DamageModifier(),
            ],
            Attacks = [
                new() {
                    Name = "Shadowflame",
                    Damage = 2,
                    Cost = 2,
                }
            ]
        };
    }
}
