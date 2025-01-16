using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class EnchantedNightcrawler: BaseCardTemplate, ICardTemplate
    {
		private class EnchantedNightcrawlerMorbidModifier : ICardModifier
		{
			// Field modifier, refresh at start of turn
			public bool ShouldRemove(GameEventInfo eventInfo)
			{
				if(eventInfo.Event == GameEvent.CREATURE_DIED)
				{
					eventInfo.Zone.QueueAnimation(new MorbidAnimation());
					eventInfo.Zone.Owner.ManaPerTurn += 1;
					return true;
				}
				return false;
			}
		}

        public override Card CreateCard() => new ()
        {
            Name = "EnchantedNightcrawler",
            MaxHealth = 6,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.EnchantedNightcrawler,
            SubTypes = [CardSubtype.FOREST, CardSubtype.SCOUT],
            Attacks = [
                new() {
                    Damage = 2,
                    Cost = 2,
                }
            ],
			Modifiers = () => [new EnchantedNightcrawlerMorbidModifier()],
        };
    }
}
