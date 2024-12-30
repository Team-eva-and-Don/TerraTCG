using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class SlimedZombie : BaseCardTemplate, ICardTemplate
    {
        private class SlimeArmorModifier : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                var anySlimes = sourceZone.Siblings
                    .Where(z=>z != sourceZone)
                    .Where(z=>z.PlacedCard?.Template.SubTypes.Contains(CardSubtype.SLIME) ?? false)
                    .Any();
                if(anySlimes)
                {
                    attack.Damage += 1;
                }
            }

            public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                if(destZone == null)
                {
                    return;
                }
                var anySlimes = destZone.Siblings
                    .Where(z=>z != destZone)
                    .Where(z=>z.PlacedCard?.Template.SubTypes.Contains(CardSubtype.SLIME) ?? false)
                    .Any();
                if(anySlimes)
                {
                    attack.Damage = Math.Max(1, attack.Damage - 1);
                }
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "SlimedZombie",
            MaxHealth = 7,
            MoveCost = 2,
            NPCID = NPCID.SlimedZombie,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.FOREST, CardSubtype.FIGHTER],
            Modifiers = () => [
                new SlimeArmorModifier(),
            ],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }
}
