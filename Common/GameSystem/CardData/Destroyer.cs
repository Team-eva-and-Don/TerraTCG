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
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class Destroyer : BaseCardTemplate, ICardTemplate
    {
        private class OnEnterSpawnSegmentsModifier : ICardModifier
        {

			private bool didSpawnSegments;
            public bool ShouldRemove(GameEventInfo eventInfo) {
				// check if the most recently drawn card is an item
                if(eventInfo.Event == GameEvent.CREATURE_ENTERED && !didSpawnSegments)
                {
					didSpawnSegments = true;
					// find two empty zones
					var siblings = eventInfo.Zone.Siblings;
					var firstEmptyZone = siblings.Where(z => z.IsEmpty()).FirstOrDefault();
					if (firstEmptyZone is Zone zone && !siblings.Any(z=>z.PlacedCard?.Template.Name == "DestroyerTail"))
					{
						zone.PlaceCard(ModContent.GetInstance<DestroyerTail>().Card);
						zone.QueueAnimation(new PlaceCardAnimation(zone.PlacedCard));
					}
					var secondEmptyZone = siblings.Where(z => z.IsEmpty()).FirstOrDefault();
					if (secondEmptyZone is Zone zone2 && !siblings.Any(z=>z.PlacedCard?.Template.Name == "DestroyerHead"))
					{
						zone2.PlaceCard(ModContent.GetInstance<DestroyerHead>().Card);
						// Copy all equipment (this might be a little unbalanced)
						zone2.QueueAnimation(new PlaceCardAnimation(zone2.PlacedCard));
					}
					return true;
                }
                return false;
            }
        }

		internal class AddProbeOnDealOrTakeDamage : ICardModifier
		{
			public bool ShouldRemove(GameEventInfo eventInfo)
			{
				if(eventInfo.Event == GameEvent.AFTER_ATTACK || eventInfo.Event == GameEvent.AFTER_RECEIVE_ATTACK)
				{
					eventInfo.Zone.Owner.Hand.Add(ModContent.GetInstance<Probe>().Card);
				}
				return false;
			}
		}

        public override Card CreateCard() => new ()
        {
            Name = "Destroyer",
            MaxHealth = 11,
			Points = 2,
            NPCID = NPCID.TheDestroyerBody,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.EVIL, CardSubtype.FIGHTER],
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawDestroyerNPC,
			Modifiers = () => [
				new OnEnterSpawnSegmentsModifier(),
				new AddProbeOnDealOrTakeDamage(),
			],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }

    internal class DestroyerHead: BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "DestroyerHead",
            MaxHealth = 11,
            MoveCost = 1,
            NPCID = NPCID.TheDestroyer,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawDestroyerNPC,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.EVIL, CardSubtype.FIGHTER],
            IsCollectable = false,
			Modifiers = () => [
				new Destroyer.AddProbeOnDealOrTakeDamage(),
			],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }

    internal class DestroyerTail : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "DestroyerTail",
            MaxHealth = 11,
            MoveCost = 1,
            NPCID = NPCID.TheDestroyerTail,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawDestroyerNPC,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.EVIL, CardSubtype.FIGHTER],
            IsCollectable = false,
			Modifiers = () => [
				new Destroyer.AddProbeOnDealOrTakeDamage(),
			],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }

    internal class Probe : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "Probe",
			IsCollectable = false,
            CardType = CardType.ITEM,
            SubTypes = [CardSubtype.CONSUMABLE, CardSubtype.BOSS, CardSubtype.ITEM],
            SelectInHandAction = (card, player) => new ApplyModifierAction(card, player),
            Skills = [ 
                new() { Cost = 0 }
            ],
            Modifiers = () => [
                new FlatDamageModifier(1, [GameEvent.AFTER_ATTACK, GameEvent.END_TURN])  {
                    Texture = TextureCache.Instance.GetStaticNPCTexture(NPCID.Probe),
                }
            ]
        };
    }
}
