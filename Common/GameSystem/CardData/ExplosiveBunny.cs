using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class ExplosiveBunny : BaseCardTemplate, ICardTemplate
    {

		private class ExplosiveBunnySelfDamageModifier : ICardModifier
		{
			public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				var critterCount = sourceZone.Siblings
					.Where(z => z.PlacedCard is PlacedCard card && card.Template.SubTypes.Contains(CardSubtype.CRITTER))
					.Count();

				attack.SelfDamage = Math.Max(0, attack.SelfDamage - critterCount);
			}
		}

        public override Card CreateCard() => new ()
        {
            Name = "ExplosiveBunny",
            MaxHealth = 8,
            CardType = CardType.CREATURE,
            NPCID = NPCID.ExplosiveBunny,
            SubTypes = [CardSubtype.FOREST, CardSubtype.SCOUT],
            Attacks = [
                new() {
                    Damage = 4,
                    Cost = 2,
                    SelfDamage = 3,
                }
            ],
			Modifiers = () => [
				new ExplosiveBunnySelfDamageModifier()
			],
        };
    }
}
