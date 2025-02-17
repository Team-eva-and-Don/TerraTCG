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
    internal class Twins : BaseCardTemplate, ICardTemplate
    {
        private class OnEnterBecomeTwinsModifier : ICardModifier
        {

            public bool ShouldRemove(GameEventInfo eventInfo) {
				// check if the most recently drawn card is an item
                if(eventInfo.Event == GameEvent.CREATURE_ENTERED)
                {
					// find two empty zones
					var siblings = eventInfo.Zone.Siblings;
					var firstEmptyZone = siblings.Where(z => z.IsEmpty()).FirstOrDefault();
					if (firstEmptyZone is Zone zone && !siblings.Any(z=>z.PlacedCard?.Template.Name == "Retinazer"))
					{
						zone.PlaceCard(ModContent.GetInstance<Retinazer>().Card);
						zone.QueueAnimation(new PlaceCardAnimation(zone.PlacedCard));
					}
					var secondEmptyZone = siblings.Where(z => z.IsEmpty()).FirstOrDefault();
					if (secondEmptyZone is Zone zone2 && !siblings.Any(z=>z.PlacedCard?.Template.Name == "Spazmatism"))
					{
						zone2.PlaceCard(ModContent.GetInstance<Spazmatism>().Card);
						// Copy all equipment (this might be a little unbalanced)
						zone2.QueueAnimation(new PlaceCardAnimation(zone2.PlacedCard));
					}

					// Remove this since it's not a playable card
					eventInfo.Zone.QueueAnimation(new RemoveCardAnimation(eventInfo.Zone.PlacedCard));
					eventInfo.Zone.PlacedCard = null;
					return true;
                }
                return false;
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "Twins",
            MaxHealth = 14,
            MoveCost = 2,
			Points = 0,
            NPCID = NPCID.None,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawNoOpNPC,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.EVIL, CardSubtype.SCOUT],
			Modifiers = () => [new OnEnterBecomeTwinsModifier()],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }
    internal class Retinazer : BaseCardTemplate, ICardTemplate
    {
        private class BuffRetinazerSturdyModifier : ICardModifier
		{
			public ModifierType Category => ModifierType.DEFENSE_BOOST;

			public int Amount => 1;

			public bool AppliesToZone(Zone zone) => 
				zone.HasPlacedCard() && zone.PlacedCard.Template.Name == "Retinazer" &&
				zone.Siblings.Any(z=> z.PlacedCard is PlacedCard card && 
					card.Template.Name == "Spazmatism" &&
					card.CurrentHealth > (card.Template.MaxHealth + 1) / 2);

			public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				if(AppliesToZone(destZone))
				{
					attack.Damage = Math.Max(1, attack.Damage - 1);
				}
			}
			public bool ShouldRemove(GameEventInfo eventInfo) => FieldModifierHelper.ShouldRemove(eventInfo, "Spazmatism");
		}

        private class BuffRetinazerRelentlessModifier : ICardModifier
		{
			public ModifierType Category => ModifierType.RELENTLESS;

			public bool AppliesToZone(Zone zone) => 
				zone.HasPlacedCard() && zone.PlacedCard.Template.Name == "Retinazer" &&
				(!zone.Siblings.Any(z=>z.PlacedCard?.Template.Name == "Spazmatism") ||
				zone.Siblings.Any(z=> z.PlacedCard is PlacedCard card && 
					card.Template.Name == "Spazmatism" &&
					card.CurrentHealth <= (card.Template.MaxHealth + 1) / 2));

			public bool ShouldRemove(GameEventInfo eventInfo) {
				if(FieldModifierHelper.ShouldRemove(eventInfo, "Spazmatism"))
				{
					return true;
				}
				if(AppliesToZone(eventInfo.Zone) && eventInfo.Event == GameEvent.AFTER_ATTACK && eventInfo.Zone?.PlacedCard is PlacedCard card && card.IsExerted)
				{
					card.IsExerted = false;
					eventInfo.Zone.QueueAnimation(new BecomeActiveAnimation(card));
					return true;
				}
				return false;
			}
		}

        public override Card CreateCard() => new ()
        {
            Name = "Retinazer",
            MaxHealth = 14,
			Points = 2,
            NPCID = NPCID.Retinazer,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawEOCNPC,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.EVIL, CardSubtype.SCOUT],
            IsCollectable = false,
            FieldModifiers = () => [ new BuffRetinazerSturdyModifier(), new BuffRetinazerRelentlessModifier(),],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }
    internal class Spazmatism : BaseCardTemplate, ICardTemplate
    {
        private class BuffSpazmatismSpikedModifier : ICardModifier
		{
			public ModifierType Category => ModifierType.SPIKED;

			public int Amount => 2;

			public bool AppliesToZone(Zone zone) => 
				zone.HasPlacedCard() && zone.PlacedCard.Template.Name == "Spazmatism" &&
				zone.Siblings.Any(z=> z.PlacedCard is PlacedCard card && 
					card.Template.Name == "Retinazer" &&
					card.CurrentHealth > (card.Template.MaxHealth + 1) / 2);

			public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				if(AppliesToZone(destZone))
				{
					attack.SelfDamage += 2;
				}
			}

			public bool ShouldRemove(GameEventInfo eventInfo) => FieldModifierHelper.ShouldRemove(eventInfo, "Retinazer");
		}

        private class BuffSpazmatismDmgModifier : ICardModifier
		{
			public bool AppliesToZone(Zone zone) => 
				zone.HasPlacedCard() && zone.PlacedCard.Template.Name == "Spazmatism" &&
				(!zone.Siblings.Any(z=>z.PlacedCard?.Template.Name == "Retinazer") ||
				zone.Siblings.Any(z=> z.PlacedCard is PlacedCard card && 
					card.Template.Name == "Retinazer" &&
					card.CurrentHealth <= (card.Template.MaxHealth + 1) / 2));

			public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				if(AppliesToZone(sourceZone))
				{
					attack.Damage += 2;
				}
			}

			public bool ShouldRemove(GameEventInfo eventInfo) => FieldModifierHelper.ShouldRemove(eventInfo, "Retinazer");
		}

        public override Card CreateCard() => new ()
        {
            Name = "Spazmatism",
            MaxHealth = 14,
			Points = 2,
            NPCID = NPCID.Spazmatism,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawEOCNPC,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.EVIL, CardSubtype.SCOUT],
            IsCollectable = false,
			FieldModifiers = () => [new BuffSpazmatismDmgModifier(), new BuffSpazmatismSpikedModifier()],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }
}
