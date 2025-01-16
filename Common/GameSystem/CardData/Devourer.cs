using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
    internal class Devourer: BaseCardTemplate, ICardTemplate
    {
		internal class MorbidDamageBoostModifier : ICardModifier
		{
			public ModifierType Category { get => ModifierType.MORBID; }
			public Asset<Texture2D> Texture { get; set; }

			public CardSubtype Source { get; set; }

			public bool ShouldRemove(GameEventInfo eventInfo)
			{
				if(eventInfo.Event == GameEvent.CREATURE_DIED)
				{
					eventInfo.Zone.QueueAnimation(new MorbidAnimation());
					foreach (var zone in eventInfo.Zone.Siblings.Where(z=>z.HasPlacedCard()))
					{
						zone.PlacedCard.AddModifiers([new FlatDamageModifier(1)]);
					}
					return true;
				}
				return false;
			}
		}

        public override Card CreateCard() => new ()
        {
            Name = "Devourer",
            MaxHealth = 6,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.DevourerHead,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawBestiaryZoneNPC,
            SubTypes = [CardSubtype.CORRUPT, CardSubtype.FIGHTER],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ],
			Modifiers = () => [new MorbidDamageBoostModifier()],
        };
    }
}
