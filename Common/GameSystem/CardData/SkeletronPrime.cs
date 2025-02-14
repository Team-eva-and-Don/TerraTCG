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
    internal class SkeletronPrime : BaseCardTemplate, ICardTemplate
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
					if (firstEmptyZone is Zone zone && !siblings.Any(z=>z.PlacedCard?.Template.Name == "SkeletronPrimeHandL"))
					{
						zone.PlaceCard(ModContent.GetInstance<SkeletronPrimeHandL>().Card);
						zone.QueueAnimation(new PlaceCardAnimation(zone.PlacedCard));
					}
					var secondEmptyZone = siblings.Where(z => z.IsEmpty()).FirstOrDefault();
					if (secondEmptyZone is Zone zone2 && !siblings.Any(z=>z.PlacedCard?.Template.Name == "SkeletronPrimeHandR"))
					{
						zone2.PlaceCard(ModContent.GetInstance<SkeletronPrimeHandR>().Card);
						// Copy all equipment (this might be a little unbalanced)
						zone2.QueueAnimation(new PlaceCardAnimation(zone2.PlacedCard));
					}
					return true;
                }
                return false;
            }
        }

		// Visual-only class that copies the buff tooltips from Skeletron Prime
		// to its hands
		private class CopyBuffTooltipModifier(ModifierType type) : ICardModifier
		{
			// FieldModifiers don't have direct access to the field they're modifying, grab it
			// when it's passed in to another method
			private List<Zone> fieldZones; 
			public ModifierType Category => type;

			public string Description { get => ""; }

			public int Amount => fieldZones?
				.Where(z => z.PlacedCard?.Template.Name == "SkeletronPrime") 
				.SelectMany(z => z.PlacedCard.CardModifiers)
				.Where(m => m.Category == Category && m.Source == CardSubtype.EQUIPMENT)
				.Select(m => m.Amount)
				.Sum()
				?? 0;

			public bool AppliesToZone(Zone zone)
			{
				fieldZones ??= zone.Siblings;
				return zone.HasPlacedCard() && zone.PlacedCard.Template.Name.StartsWith("SkeletronPrimeHand") &&
				zone.Siblings.Any(z => z.PlacedCard?.Template.Name == "SkeletronPrime") && Amount > 0;
			}

			public bool ShouldRemove(GameEventInfo eventInfo) =>
				FieldModifierHelper.ShouldRemove(eventInfo, "SkeletronPrime");
		}

        public override Card CreateCard() => new ()
        {
            Name = "SkeletronPrime",
            MaxHealth = 15,
			Points = 2,
            NPCID = NPCID.SkeletronPrime,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.EVIL, CardSubtype.FIGHTER],
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawSkeletronPrimeNPC,
			Modifiers = () => [new OnEnterSpawnHandsModifier(), new ReduceDamageModifier(1)],
			FieldModifiers = () => Enum.GetValues(typeof(ModifierType)).OfType<ModifierType>()
				.Select(m=>new CopyBuffTooltipModifier(m) as ICardModifier).ToList(),
            Attacks = [
                new() {
                    Damage = 1,
                    Cost = 4,
                }
            ]
        };
    }
    internal class SkeletronPrimeHandL: BaseCardTemplate, ICardTemplate
    {
		// For each buff modification method, copy any equipment buffs from the relevant Skeletron Prime
		internal class SkeletronEquipmentBuffCopyModifier : ICardModifier
		{
			private static IEnumerable<ICardModifier> SkeletronEquips(Zone zone) =>
				zone.Siblings.Where(z => z.PlacedCard?.Template.Name == "SkeletronPrime")
				.SelectMany(z => z.PlacedCard.CardModifiers)
				.Where(m => m.Source == CardSubtype.EQUIPMENT);

			// These are the only 3 relevant modifiers for equipment based modifiers
			// (Famous last words)

			public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				foreach(var modifier in SkeletronEquips(sourceZone))
				{
					modifier.ModifyAttack(ref attack, sourceZone, destZone);
				}
			}

			public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				foreach(var modifier in SkeletronEquips(destZone))
				{
					modifier.ModifyIncomingAttack(ref attack, sourceZone, destZone);
				}
			}

			public void ModifyZoneSelection(Zone sourceZone, Zone endZone, ref List<Zone> destZones)
			{
				foreach(var modifier in SkeletronEquips(sourceZone))
				{
					modifier.ModifyZoneSelection(sourceZone, endZone, ref destZones);
				}
			}
			public bool ShouldRemove(GameEventInfo eventInfo) {
				// This results in somewhat weird behavior like Relentless only giving one extra
				// attack among all 3 parts of the card
				foreach(var modifier in SkeletronEquips(eventInfo.Zone))
				{
					modifier.ShouldRemove(eventInfo);
				}
				return false;
			}
		}

        public override Card CreateCard() => new ()
        {
            Name = "SkeletronPrimeHandL",
            MaxHealth = 9,
            NPCID = NPCID.PrimeLaser,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawRotatedZoneNPC,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.EVIL, CardSubtype.SCOUT],
            IsCollectable = false,
            Modifiers = () => [ 
				new EvasiveModifier(), 
				new SkeletronEquipmentBuffCopyModifier() 
			],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }
    internal class SkeletronPrimeHandR : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "SkeletronPrimeHandR",
            MaxHealth = 10,
            NPCID = NPCID.PrimeVice,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawFlippedZoneNPC,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.EVIL, CardSubtype.SCOUT],
            IsCollectable = false,
            Modifiers = () => [
				new RelentlessModifier(), 
				new SkeletronPrimeHandL.SkeletronEquipmentBuffCopyModifier() 
            ],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }
}
