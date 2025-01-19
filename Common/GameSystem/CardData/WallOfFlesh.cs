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
    internal class WallOfFlesh : BaseCardTemplate, ICardTemplate
    {
        private class IncrementPowerModifier : ICardModifier
        {

			private int damageBoost;
			private bool didIncrementThisTurn;

			public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				// no-op
				attack.Damage += damageBoost;
			}

            public bool ShouldRemove(GameEventInfo eventInfo) {
                if(eventInfo.IsMyTurn && !didIncrementThisTurn  && eventInfo.Event == GameEvent.END_TURN)
                {

					// TODO for clarity's sake, would prefer not to account for both the boss and the
					// helper in one modifier
					var leechCount = eventInfo.Zone.Siblings.Where(z => z.PlacedCard?.Template.Name == "Leech").Count();
					didIncrementThisTurn = true;
					eventInfo.Zone.PlacedCard.Heal(leechCount + 1);
					damageBoost += leechCount + 1;
                } else if(eventInfo.Event == GameEvent.START_TURN)
				{
					didIncrementThisTurn = false;
				}
                return false;
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "WallOfFlesh",
            MaxHealth = 15,
            MoveCost = 2,
            NPCID = NPCID.WallofFlesh,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawWOFNPC,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.EVIL, CardSubtype.DEFENDER],
            Modifiers = () => [
                new IncrementPowerModifier(),
            ],
            Attacks = [
                new() {
                    Damage = 2,
                    Cost = 3,
                }
            ]
        };
    }
}
