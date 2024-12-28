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
    internal class MotherSlime : BaseCardTemplate, ICardTemplate
    {
        private class SlimeAttackModifier : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                // no-op
                var slimeCount = sourceZone.Siblings
                    .Where(z=>z.PlacedCard?.Template.SubTypes.Contains(CardSubtype.SLIME) ?? false)
                    .Count();
                attack.Damage += slimeCount;
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "MotherSlime",
            MaxHealth = 8,
            MoveCost = 2,
            NPCID = NPCID.MotherSlime,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.SLIME, CardSubtype.FIGHTER],
            DrawZoneNPC = CardOverlayRenderer.Instance.DrawSlimeNPC(1f, Color.White * 0.8f),
            Modifiers = [
                new SlimeAttackModifier(),
            ],
            Attacks = [
                new() {
                    Name = "Slimed",
                    Damage = 1,
                    Cost = 3,
                }
            ]
        };
    }
}
