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
    internal class GiantFlyingFox : BaseCardTemplate, ICardTemplate
    {
		private class BatBuffModifier : ICardModifier
		{
			public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				bool isBat = sourceZone.PlacedCard is PlacedCard card &&
					card.Template.Name.Contains("Bat");
				if(isBat)
				{
					attack.Damage += 1;
					attack.Cost = Math.Max(1, attack.Cost - 1);
				}
			}

			public bool ShouldRemove(GameEventInfo eventInfo) =>
				FieldModifierHelper.ShouldRemove(eventInfo, "GiantFlyingFox");
		}

        public override Card CreateCard() => new ()
        {
            Name = "GiantFlyingFox",
            MaxHealth = 9,
            NPCID = NPCID.GiantFlyingFox,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.EXPERT, CardSubtype.JUNGLE, CardSubtype.SCOUT],
			CanPromote = (zone, card) => zone.PlacedCard?.Template.Name.Contains("Bat") ?? false,
            FieldModifiers = () => [
                new BatBuffModifier(),
            ],
			Modifiers = () => [
				new EvasiveModifier(),
			],
            Attacks = [
                new() {
                    Damage = 4,
                    Cost = 3,
                }
            ]
        };
    }
}
