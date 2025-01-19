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
    internal class Skeletron : BaseCardTemplate, ICardTemplate
    {
        private class OnEnterSpawnHandsModifier : ICardModifier
        {

			private bool didSpawnHands;
            public bool ShouldRemove(GameEventInfo eventInfo) {
				// check if the most recently drawn card is an item
                if(eventInfo.Event == GameEvent.CREATURE_ENTERED && !didSpawnHands)
                {
					didSpawnHands = true;
					// find two empty zones
					var siblings = eventInfo.Zone.Siblings;
					var firstEmptyZone = siblings.Where(z => z.IsEmpty()).FirstOrDefault();
					if (firstEmptyZone is Zone zone && !siblings.Any(z=>z.PlacedCard?.Template.Name == "SkeletronHandL"))
					{
						zone.PlaceCard(ModContent.GetInstance<SkeletronHandL>().Card);
						zone.QueueAnimation(new PlaceCardAnimation(zone.PlacedCard));
					}
					var secondEmptyZone = siblings.Where(z => z.IsEmpty()).FirstOrDefault();
					if (secondEmptyZone is Zone zone2 && !siblings.Any(z=>z.PlacedCard?.Template.Name == "SkeletronHandR"))
					{
						zone2.PlaceCard(ModContent.GetInstance<SkeletronHandR>().Card);
						// Copy all equipment (this might be a little unbalanced)
						zone2.QueueAnimation(new PlaceCardAnimation(zone2.PlacedCard));
					}
					return true;
                }
                return false;
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "Skeletron",
            MaxHealth = 11,
            MoveCost = 2,
            NPCID = NPCID.SkeletronHead,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.CAVERN, CardSubtype.FIGHTER],
			Modifiers = () => [new OnEnterSpawnHandsModifier()],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }
    internal class SkeletronHandL: BaseCardTemplate, ICardTemplate
    {
        private class BuffSkeletronDamageModifier : ICardModifier
		{
			public bool AppliesToZone(Zone zone) =>
				zone.PlacedCard?.Template.Name == "Skeletron" &&
				zone.Siblings.Any(z => z.Role == ZoneRole.OFFENSE && z.PlacedCard?.Template.Name == "SkeletronHandL");
			
			public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				// no-op
				if(AppliesToZone(sourceZone))
				{
					attack.Damage += 2;
				}
			}

			// FieldModifier, remove/reapply on turn start
			public bool ShouldRemove(GameEventInfo eventInfo) => eventInfo.Event == GameEvent.START_TURN;
		}

        public override Card CreateCard() => new ()
        {
            Name = "SkeletronHandL",
            MaxHealth = 6,
            MoveCost = 1,
			Priority = 10,
            NPCID = NPCID.SkeletronHand,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawRotatedZoneNPC,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.SCOUT],
            IsCollectable = false,
            FieldModifiers = () => [ new BuffSkeletronDamageModifier() ],
            Attacks = [
                new() {
                    Damage = 2,
                    Cost = 1,
                }
            ]
        };
    }
    internal class SkeletronHandR : BaseCardTemplate, ICardTemplate
    {
        private class BuffSkeletronDefenseModifier : ICardModifier
		{
			public bool AppliesToZone(Zone zone) =>
				zone.PlacedCard?.Template.Name == "Skeletron" &&
				zone.Siblings.Any(z => z.Role == ZoneRole.OFFENSE && z.PlacedCard?.Template.Name == "SkeletronHandR");

			public int Amount => 2;
			public ModifierType Category => ModifierType.DEFENSE_BOOST;

			public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				// no-op
				if(AppliesToZone(destZone))
				{
					attack.Damage = Math.Max(1, attack.Damage - Amount);
				}
			}


			// FieldModifier, remove on turn start
			public bool ShouldRemove(GameEventInfo eventInfo) => eventInfo.Event == GameEvent.START_TURN;
		}

        public override Card CreateCard() => new ()
        {
            Name = "SkeletronHandR",
            MaxHealth = 6,
            MoveCost = 1,
            NPCID = NPCID.SkeletronHand,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawFlippedZoneNPC,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.SCOUT],
            IsCollectable = false,
			FieldModifiers = () => [new BuffSkeletronDefenseModifier()],
			Priority = 10,
            Modifiers = () => [
                new ReduceDamageModifier(1),
            ],
            Attacks = [
                new() {
                    Damage = 1,
                    Cost = 1,
                }
            ]
        };
    }
}
