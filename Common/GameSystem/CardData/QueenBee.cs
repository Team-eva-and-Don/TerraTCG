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
        private class QueenSlimePoisonBoost : ICardModifier
        {
			private bool didApplyThisTurn = false;
            public bool ShouldRemove(GameEventInfo eventInfo)
			{
				if(eventInfo.Event == GameEvent.START_TURN)
				{
					return true;
				} else if (eventInfo.Event == GameEvent.END_TURN && !didApplyThisTurn)
				{
					didApplyThisTurn = true;
					// Deal 1 damage to every poisoned enemy on the opponent's field
					// (the buff applies to the owner's field, which is confusing)

					var opponentZones = eventInfo.Zone.Owner.Opponent.Field.Zones
						.Where(z => z.PlacedCard?.CardModifiers.Any(m => m.Category == ModifierType.POISON) ?? false);

					foreach (var zone in opponentZones)
					{
						zone.PlacedCard.CurrentHealth -= 1;
					}
				}
				return false;
			}
        }

        public override Card CreateCard() => new ()
        {
            Name = "QueenBee",
            MaxHealth = 11,
            MoveCost = 2,
            Points = 2,
            NPCID = NPCID.QueenBee,
            CardType = CardType.CREATURE,
            Role = ZoneRole.DEFENSE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.JUNGLE, CardSubtype.FIGHTER],
            DrawZoneNPC = CardOverlayRenderer.Instance.DrawQueenBeeNPC,
			FieldModifiers = () => [new QueenSlimePoisonBoost()],
            Attacks = [
                new() {
                    Damage = 4,
                    Cost = 4,
                }
            ]
        };
    }
}
