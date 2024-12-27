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
    internal class Pinky : ModSystem, ICardTemplate
    {
        private class SlimeCostModifier : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                // no-op
                var slimeCount = sourceZone.Siblings
                    .Where(z=>z.PlacedCard?.Template.SubTypes.Contains(CardSubtype.SLIME) ?? false)
                    .Count();
                attack.Cost = Math.Max(1, attack.Cost - slimeCount);
            }
        }

        public Card CreateCard() => new ()
        {
            Name = "Pinky",
            MaxHealth = 8,
            MoveCost = 2,
            NPCID = NPCID.BlueSlime,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.FOREST, CardSubtype.SLIME, CardSubtype.FIGHTER],
            DrawZoneNPC = CardOverlayRenderer.Instance.DrawSlimeNPC(0.5f, new Color(250, 30, 90, 180)),
            Modifiers = [
                new SlimeCostModifier(),
            ],
            Attacks = [
                new() {
                    Damage = 4,
                    Cost = 5,
                }
            ]
        };
    }
}
