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
    internal class GoblinScout : ModSystem, ICardTemplate
    {
        internal class DamageModifier : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                var itemCount = sourceZone.Owner.Game.CurrentTurn.UsedItemCount;
                attack.Damage += itemCount;
            }
        }

        public Card CreateCard() => new ()
        {
            Name = "GoblinScout",
            MaxHealth = 7,
            MoveCost = 1,
            NPCID = NPCID.GoblinScout,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.FOREST, CardSubtype.SCOUT],
            Modifiers = [
                new ZealousModifier(),
                new DamageModifier(),
            ],
            Attacks = [
                new() {
                    Name = "Scout",
                    Damage = 1,
                    Cost = 1,
                }
            ]
        };
    }
}
