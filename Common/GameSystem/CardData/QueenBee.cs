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
    internal class QueenBee : BaseCardTemplate, ICardTemplate
    {
        private class QueenBeePoisonBoost : ICardModifier
        {
			public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				if(!(sourceZone.PlacedCard?.CardModifiers.Any(m=>m.Category == ModifierType.POISON) ?? true)) {
					var localAttack = attack;
					attack.SourceModifiers = (zone) => [new PoisonModifier(), .. localAttack.SourceModifiers?.Invoke(zone) ?? []];
				}
			}

			// Field modifier, refresh at start of turn
			public bool ShouldRemove(GameEventInfo eventInfo) => FieldModifierHelper.ShouldRemove(eventInfo, "QueenBee");
        }

        public override Card CreateCard() => new ()
        {
            Name = "QueenBee",
            MaxHealth = 8,
            MoveCost = 2,
            Points = 2,
            NPCID = NPCID.QueenBee,
            CardType = CardType.CREATURE,
			Role = ZoneRole.DEFENSE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.JUNGLE, CardSubtype.FIGHTER],
            DrawZoneNPC = CardOverlayRenderer.Instance.DrawQueenBeeNPC,
			FieldModifiers = () => [new QueenBeePoisonBoost()],
            Attacks = [
                new() {
                    Damage = 4,
                    Cost = 4,
                }
            ]
        };
    }
}
