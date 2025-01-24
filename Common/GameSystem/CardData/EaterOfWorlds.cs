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
    internal class EaterOfWorlds : BaseCardTemplate, ICardTemplate
    {
        private class HalfHealthTransformModifier : ICardModifier
        {

			private bool didTransform;

            public bool ShouldRemove(GameEventInfo eventInfo) {
				bool atHalfHealth = eventInfo.Zone.PlacedCard is PlacedCard card && card.CurrentHealth <= (card.Template.MaxHealth + 1) / 2;
                if(atHalfHealth && !didTransform)
                {
					didTransform = true;
                    eventInfo.Zone.PromoteCard(ModContent.GetInstance<EaterSegment1>().CreateCard());
					eventInfo.Zone.PlacedCard.CurrentHealth = eventInfo.Zone.PlacedCard.Template.MaxHealth;
					// find an empty zone
					var firstEmptyZone = eventInfo.Zone.Siblings.Where(z => z.IsEmpty()).FirstOrDefault();
					if (firstEmptyZone is Zone zone)
					{
						zone.PlaceCard(ModContent.GetInstance<EaterSegment2>().CreateCard());
						// Copy all equipment (this might be a little unbalanced)
						zone.PlacedCard.AddModifiers(eventInfo.Zone.PlacedCard.CardModifiers.Where(m => m.Source == CardSubtype.EQUIPMENT).ToList());
						zone.QueueAnimation(new PlaceCardAnimation(zone.PlacedCard));
					}
                }
                return false;
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "EaterOfWorlds",
            MaxHealth = 11,
            MoveCost = 2,
			Points = 2,
            NPCID = NPCID.EaterofWorldsHead,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawBestiaryZoneNPC,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.EVIL, CardSubtype.FIGHTER],
            Modifiers = () => [
                new LifestealModifier(1),
                new SpikedModifier(1),
                new HalfHealthTransformModifier(),
            ],
            Attacks = [
                new() {
                    Damage = 4,
                    Cost = 3,
                }
            ]
        };
    }
    internal class EaterSegment1 : BaseCardTemplate, ICardTemplate
    {
		private class EaterSegment1MorbidModifier : ICardModifier
		{
			public ModifierType Category { get => ModifierType.MORBID; }

			public bool ShouldRemove(GameEventInfo eventInfo)
			{
				if(eventInfo.Event == GameEvent.CREATURE_DIED)
				{
					eventInfo.Zone.QueueAnimation(new MorbidAnimation());
					foreach (var zone in eventInfo.Zone.Siblings.Where(z=>z.HasPlacedCard()))
					{
						zone.PlacedCard.AddModifiers([new SpikedModifier(2)]);
					}
					return true;
				}
				return false;
			}
		}
        public override Card CreateCard() => new ()
        {
            Name = "EaterSegment1",
            MaxHealth = 5,
            MoveCost = 2,
            NPCID = NPCID.EaterofWorldsHead,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.EVIL, CardSubtype.SCOUT],
            IsCollectable = false,
            Modifiers = () => [
				new EaterSegment1MorbidModifier(),
                new SpikedModifier(2),
            ],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }

    internal class EaterSegment2 : BaseCardTemplate, ICardTemplate
    {
		private class EaterSegment2MorbidModifier : ICardModifier
		{
			public ModifierType Category { get => ModifierType.MORBID; }

			public bool ShouldRemove(GameEventInfo eventInfo)
			{
				if(eventInfo.Event == GameEvent.CREATURE_DIED)
				{
					eventInfo.Zone.QueueAnimation(new MorbidAnimation());
					foreach (var zone in eventInfo.Zone.Siblings.Where(z=>z.HasPlacedCard()))
					{
						zone.PlacedCard.AddModifiers([new LifestealModifier(2)]);
					}
					return true;
				}
				return false;
			}
		}
        public override Card CreateCard() => new ()
        {
            Name = "EaterSegment2",
            MaxHealth = 5,
            MoveCost = 2,
            NPCID = NPCID.EaterofWorldsHead,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.EVIL, CardSubtype.SCOUT],
            IsCollectable = false,
            Modifiers = () => [
				new EaterSegment2MorbidModifier(),
                new LifestealModifier(2),
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
