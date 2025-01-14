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
    internal class BrainOfCthulhu : BaseCardTemplate, ICardTemplate
    {
        private class BOCBloodBoost : ICardModifier
        {
			public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				if(destZone?.PlacedCard?.CardModifiers.Any(c=>c.Category == ModifierType.BLEEDING) ?? false)
				{
					attack.Damage += 2;
				}
			}
        }

        public override Card CreateCard() => new ()
        {
            Name = "BrainOfCthulhu",
            MaxHealth = 9,
            MoveCost = 2,
            Points = 2,
            NPCID = NPCID.BrainofCthulhu,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.CRIMSON, CardSubtype.FIGHTER],
            DrawZoneNPC = CardOverlayRenderer.Instance.DrawBOCNPC,
			Modifiers = () => [new BOCBloodBoost(), new ZealousModifier()],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }
}
