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
    internal class BatWings : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "BatWings",
            CardType = CardType.ITEM,
            SubTypes = [CardSubtype.EQUIPMENT, CardSubtype.ITEM],
            SelectInHandAction = (card, player) => new ApplyModifierAction(card, player),
            ShouldTarget = z => true,
			CanTargetZone = z => z.HasPlacedCard() && z.PlacedCard.Template.Name.Contains("Bat"),
            Skills = [
                new() { Cost = 2 }
            ],
            Modifiers = () => [
                new RelentlessModifier()  {
                    Texture = TextureCache.Instance.GetItemTexture(ItemID.BatWings),
                },
				new LifestealModifier(1)
            ]
        };
    }
}
