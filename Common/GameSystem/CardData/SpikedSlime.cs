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
    internal class SpikedSlime: BaseCardTemplate, ICardTemplate
    {
		private class SpikedSlimeBossBuffModifier : ICardModifier
		{
			public ModifierType Category => ModifierType.SPIKED;
			public int Amount => 0;
			public bool AppliesToZone(Zone zone) => zone.PlacedCard?.Template.Name == "KingSlime";

			public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				// no-op
				if(AppliesToZone(destZone))
				{
					attack.SelfDamage += 2;
				}
			}

			// Field modifier, refresh at start of turn
			public bool ShouldRemove(GameEventInfo eventInfo) => eventInfo.Event == GameEvent.START_TURN;
		}

        public override Card CreateCard() => new ()
        {
            Name = "SpikedSlime",
            MaxHealth = 6,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.SlimeSpiked,
            SubTypes = [CardSubtype.FOREST, CardSubtype.SLIME, CardSubtype.DEFENDER],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ],
			Modifiers = () => [new SpikedModifier(2)],
			FieldModifiers = () => [new SpikedSlimeBossBuffModifier()],
        };
    }
}
