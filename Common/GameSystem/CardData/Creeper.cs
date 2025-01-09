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
    internal class Creeper: BaseCardTemplate, ICardTemplate
    {
		private class CreeperBossBuffModifier : ICardModifier
		{
			public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				// no-op
				if(sourceZone.PlacedCard?.Template.Name == "BrainOfCthulhu")
				{
					var localAttack = attack;
					attack.TargetModifiers = (zone) => [new BleedModifier(1), .. localAttack.TargetModifiers?.Invoke(zone) ?? []];
				}
			}

			// Field modifier, refresh at start of turn
			public bool ShouldRemove(GameEventInfo eventInfo) => eventInfo.Event == GameEvent.START_TURN;
		}

        public override Card CreateCard() => new ()
        {
            Name = "Creeper",
            MaxHealth = 6,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Creeper,
            SubTypes = [CardSubtype.CRIMSON, CardSubtype.SCOUT],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ],
			FieldModifiers = () => [new CreeperBossBuffModifier()],
        };
    }
}
