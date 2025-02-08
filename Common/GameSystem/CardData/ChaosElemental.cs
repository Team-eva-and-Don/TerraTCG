using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class ChaosElemental : BaseCardTemplate, ICardTemplate
    {
        private void ChaosWarp(GamePlayer player, Zone zone, Zone targetZone)
        {
			zone.PlacedCard.IsExerted = false; // Allow warping multiple times per turn
			var toSwap = targetZone.PlacedCard;

			// Swap!
			targetZone.PlacedCard = zone.PlacedCard;
			zone.PlacedCard = toSwap;

			targetZone.QueueAnimation(new RemoveCardAnimation(toSwap));
			targetZone.QueueAnimation(new PlaceCardAnimation(targetZone.PlacedCard));

			zone.QueueAnimation(new RemoveCardAnimation(targetZone.PlacedCard));
			zone.QueueAnimation(new PlaceCardAnimation(toSwap));

        }

        public override Card CreateCard() => new ()
        {
            Name = "ChaosElemental",
            MaxHealth = 7,
            CardType = CardType.CREATURE,
            NPCID = NPCID.ChaosElemental,
            SubTypes = [CardSubtype.HALLOWED, CardSubtype.FIGHTER],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ],
            Skills = [
                new() {
                    Cost = 1,
                    SkillType = ActionType.TARGET_ALLY,
                    DoSkill = ChaosWarp,
                }
            ]
        };
    }
}
