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
    internal class ServantOfCthulhu: BaseCardTemplate, ICardTemplate
    {
		private class SOCBossBuffModifier : ICardModifier
		{
			public ModifierType Category => ModifierType.DEFENSE_BOOST;

			public int Amount => 1;

			public bool AppliesToZone(Zone zone) => zone.PlacedCard?.Template.Name == "EyeOfCthulhu" &&
				zone.PlacedCard.CurrentHealth <= (zone.PlacedCard.Template.MaxHealth + 1) /2;

			public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				// no-op
				if(AppliesToZone(destZone))
				{
					attack.Damage = Math.Max(1, attack.Damage - Amount);
				}
			}

			// Field modifier, refresh at start of turn
			public bool ShouldRemove(GameEventInfo eventInfo) => FieldModifierHelper.ShouldRemove(eventInfo, "ServantOfCthulhu");
		}

        public override Card CreateCard() => new ()
        {
            Name = "ServantOfCthulhu",
            MaxHealth = 6,
            MoveCost = 2,
			Priority = 10,
            CardType = CardType.CREATURE,
            NPCID = NPCID.ServantofCthulhu,
            SubTypes = [CardSubtype.FOREST, CardSubtype.CRITTER],
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawFlippedZoneNPC,
            Attacks = [
                new() {
                    Damage = 1,
                    Cost = 1,
                }
            ],
			Modifiers = () => [new EvasiveModifier()],
			FieldModifiers = () => [new SOCBossBuffModifier()],
        };
    }
}
