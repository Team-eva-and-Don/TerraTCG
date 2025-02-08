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
    internal class EnchantedSword : BaseCardTemplate, ICardTemplate
    {
		private class EnchantedSwordHallowModifier : ICardModifier
		{
			public bool AppliesToZone(Zone zone) => zone.Index % 3 == 1;

			public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				if(AppliesToZone(sourceZone))
				{
					attack.Damage += 1;
				}
			}

			// Field modifier, refresh at start of turn
			public bool ShouldRemove(GameEventInfo eventInfo) => FieldModifierHelper.ShouldRemove(eventInfo, "EnchantedSword");
		}

        public override Card CreateCard() => new ()
        {
            Name = "EnchantedSword",
            MaxHealth = 7,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.EnchantedSword,
            SubTypes = [CardSubtype.HALLOWED, CardSubtype.FIGHTER],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ],
			FieldModifiers = () => [new EnchantedSwordHallowModifier()],
        };
    }
}
