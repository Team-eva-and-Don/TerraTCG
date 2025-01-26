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
				attack.Damage = damageBoost;
			}

            public bool ShouldRemove(GameEventInfo eventInfo) {
                if(eventInfo.IsMyTurn && !didIncrementThisTurn  && eventInfo.Event == GameEvent.END_TURN)
                {

					// TODO for clarity's sake, would prefer not to account for both the boss and the
					// helper in one modifier
					var leechCount = eventInfo.Zone.Siblings.Where(z => z.PlacedCard?.Template.Name == "Leech").Count();
					var eyesCount = eventInfo.Zone.Siblings.Where(z => z.PlacedCard?.Template.Name == "WallOfFleshEye").Count();
					didIncrementThisTurn = true;
					eventInfo.Zone.PlacedCard.Heal(leechCount + 1);
					damageBoost += eyesCount;
                } else if(eventInfo.Event == GameEvent.START_TURN)
				{
					didIncrementThisTurn = false;
				}
                return false;
            }
        }
        private class OnEnterSpawnEyeModifier : ICardModifier
        {

			private bool didSpawnEyes;
            public bool ShouldRemove(GameEventInfo eventInfo) {
                if(eventInfo.Event == GameEvent.CREATURE_ENTERED && !didSpawnEyes)
                {
					didSpawnEyes = true;
					// find two empty zones
					var siblings = eventInfo.Zone.Siblings;
					var missingEyeCount = 2 - siblings.Where(z => z.PlacedCard?.Template.Name == "WallOfFleshEye").Count();
					var emptyZones = siblings.Where(z => z.IsEmpty() && z.Role == ZoneRole.OFFENSE).Take(missingEyeCount);
					foreach(var zone in emptyZones)
					{
						zone.PlaceCard(ModContent.GetInstance<WallOfFleshEye>().Card);
						zone.QueueAnimation(new PlaceCardAnimation(zone.PlacedCard));
					}
					return true;
                }
                return false;
            }
        }

        private class OnLeaveKillEyesModifier : ICardModifier
        {
            public bool ShouldRemove(GameEventInfo eventInfo) {
                if(eventInfo.Event == GameEvent.CREATURE_DIED)
                {
					// find two empty zones
					var siblings = eventInfo.Zone.Siblings;
					var eyeZones = siblings.Where(z => !z.IsEmpty() && z.PlacedCard.Template.Name == "WallOfFleshEye");
					foreach(var zone in eyeZones)
					{
						zone.PlacedCard.CurrentHealth = 0;
					}
					return true;
                }
                return false;
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "WallOfFlesh",
            MaxHealth = 15,
            MoveCost = 2,
			Points = 2,
            NPCID = NPCID.WallofFlesh,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawWOFNPC,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.EVIL, CardSubtype.DEFENDER],
            Modifiers = () => [
                new IncrementPowerModifier(),
				new OnEnterSpawnEyeModifier(),
				new OnLeaveKillEyesModifier(),
            ],
            Attacks = [
                new() {
                    Damage = -1,
                    Cost = 4,
                }
            ]
        };
    }

    internal class WallOfFleshEye : BaseCardTemplate, ICardTemplate
	{

		private class CantAttackModifier : ICardModifier
		{
			public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				if(destZone != null)
				{
					attack.Cost = 999;
				}
			}

		}

        public override Card CreateCard() => new ()
        {
            Name = "WallOfFleshEye",
            MaxHealth = 6,
            MoveCost = 2,
			Points = 1,
            NPCID = NPCID.WallofFlesh,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawNoOpNPC,
            CardType = CardType.CREATURE,
			IsCollectable = false,
            SubTypes = [CardSubtype.BOSS, CardSubtype.EVIL, CardSubtype.DEFENDER],
            Modifiers = () => [
				new CantAttackModifier(),
				new ReduceDamageModifier(1),
            ],
            Attacks = [
                new() {
                    Damage = -2,
                    Cost = 0,
                }
            ]
        };
	}
}
