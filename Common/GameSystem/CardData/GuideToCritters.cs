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
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class GuideToCritters : BaseCardTemplate, ICardTemplate
    {
		private class GuideToCrittersTargetModifier : ICardModifier
		{
			public Asset<Texture2D> Texture { get; set; }

			public Card SourceCard { get; set; }

            public void ModifyIncomingZoneSelection(Zone sourceZone, Zone endZone, ref List<Zone> destZones)
            {
                // Only allow attacks against the equipped creature
                if(!sourceZone.IsEmpty())
                {
					destZones = destZones.Where(z => z.PlacedCard is PlacedCard card && !card.Template.SubTypes.Contains(CardSubtype.CRITTER)).ToList();
					if(destZones.Count == 0)
					{
						destZones = [sourceZone];
					}
                }
            }

		}

        public override Card CreateCard() => new ()
        {
            Name = "GuideToCritters",
            CardType = CardType.ITEM,
            SubTypes = [CardSubtype.EQUIPMENT, CardSubtype.ITEM],
            SelectInHandAction = (card, player) => new ApplyModifierAction(card, player),
            ShouldTarget = z => true,
			CanTargetZone = z => z.HasPlacedCard() && !z.PlacedCard.Template.SubTypes.Contains(CardSubtype.CRITTER),
            Skills = [ 
                new() { Cost = 2 }
            ],
            Modifiers = () => [
                new GuideToCrittersTargetModifier()  {
                    Texture = TextureCache.Instance.GetItemTexture(ItemID.DontHurtCrittersBook),
                }
            ]
        };
    }
}
