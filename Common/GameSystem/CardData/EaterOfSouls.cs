using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class EaterOfSouls: BaseCardTemplate, ICardTemplate
    {
		private class EaterOfSoulsMorbidModifier : ICardModifier
		{
			public ModifierType Category { get => ModifierType.MORBID; }

			public bool ShouldRemove(GameEventInfo eventInfo)
			{
				if(eventInfo.Event == GameEvent.CREATURE_DIED)
				{
					eventInfo.Zone.QueueAnimation(new MorbidAnimation());
					foreach (var zone in eventInfo.Zone.Siblings.Where(z=>z.HasPlacedCard()))
					{
						zone.PlacedCard.AddModifiers([new ReduceDamageModifier(1)]);
					}
					return true;
				}
				return false;
			}
		}

        public override Card CreateCard() => new ()
        {
            Name = "EaterOfSouls",
            MaxHealth = 6,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.EaterofSouls,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawFlippedZoneNPC,
            SubTypes = [CardSubtype.EVIL, CardSubtype.FIGHTER],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ],
			Modifiers = () => [new EaterOfSoulsMorbidModifier()],
        };
    }
}
