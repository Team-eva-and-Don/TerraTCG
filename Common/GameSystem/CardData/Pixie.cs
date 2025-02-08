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
    internal class Pixie: BaseCardTemplate, ICardTemplate
    {
		private class PixieHallowedModifier : ICardModifier
		{
			public bool ShouldRemove(GameEventInfo eventInfo) {
				var centralZone = eventInfo.Zone.Siblings.Where(z => z.Index == 1).First();
				if(centralZone.HasPlacedCard())
				{
					centralZone.PlacedCard.Heal(1);
				}
				return false;
			}
		}

        public override Card CreateCard() => new ()
        {
            Name = "Pixie",
            MaxHealth = 7,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Pixie,
            SubTypes = [CardSubtype.HALLOWED, CardSubtype.DEFENDER],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ],
			Modifiers = () => [new PixieHallowedModifier()],
        };
    }
}
