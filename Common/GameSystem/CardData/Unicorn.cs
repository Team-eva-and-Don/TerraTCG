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
    internal class Unicorn : BaseCardTemplate, ICardTemplate
    {
		private class UnicornHallowedModifier : ICardModifier
		{
			public bool AppliesToZone(Zone zone) => zone.Index % 3 == 1;

			public void ModifyCardEntrance(Zone sourceZone) 
			{
				// Having cards enter unpaused on turn 1 screws up a 
				// bunch of enemy decision-making.
				if(AppliesToZone(sourceZone) && sourceZone.Owner.Game.CurrentTurn.TurnCount > 1)
				{
					sourceZone.PlacedCard.IsExerted = false;
				}
			}

			// Field modifier, refresh at start of turn
			public bool ShouldRemove(GameEventInfo eventInfo) => FieldModifierHelper.ShouldRemove(eventInfo, "Unicorn");
		}

		private class UnicornOnEnterModifier : ICardModifier
		{

			public void ModifyCardEntrance(Zone sourceZone) 
			{
				// Having cards enter unpaused on turn 1 screws up a 
				// bunch of enemy decision-making.
				var centerZone = sourceZone.Siblings.Where(z => z.Index == 1).First();
				if(sourceZone.Owner.Game.CurrentTurn.TurnCount > 1 && centerZone.HasPlacedCard())
				{
					centerZone.PlacedCard.IsExerted = false;
				}
			}

			public bool ShouldRemove(GameEventInfo eventInfo) => true;
		}

        public override Card CreateCard() => new ()
        {
            Name = "Unicorn",
            MaxHealth = 7,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Unicorn,
            SubTypes = [CardSubtype.HALLOWED, CardSubtype.FIGHTER],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ],
			Modifiers = () => [new UnicornOnEnterModifier()],
			FieldModifiers = () => [new UnicornHallowedModifier()],
        };
    }
}
