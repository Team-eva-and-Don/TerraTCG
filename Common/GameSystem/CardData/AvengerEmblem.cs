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
    internal class AvengerEmblem : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "AvengerEmblem",
            CardType = CardType.ITEM,
            SubTypes = [CardSubtype.EQUIPMENT, CardSubtype.ITEM],
            SelectInHandAction = (card, player) => new ApplyModifierAction(card, player),
            ShouldTarget = z => true,
            Skills = [ 
                new() { Cost = 1 }
            ],
            Modifiers = () => [
				new AngryBones.EquipCostModifier() {
					Texture = TextureCache.Instance.GetItemTexture(ItemID.AvengerEmblem),
				}
            ]
        };
    }
}
