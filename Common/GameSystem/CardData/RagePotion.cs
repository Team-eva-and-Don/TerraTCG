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
    internal class RagePotion: BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "RagePotion",
            CardType = CardType.ITEM,
            SubTypes = [CardSubtype.CONSUMABLE, CardSubtype.ITEM],
            SelectInHandAction = (card, player) => new ApplyModifierAction(card, player),
            Skills = [ // TODO this is wonky, but item texts are drawn using the skill template
                new() { Cost = 2 }
            ],
            Modifiers = () => [
                new FlatDamageModifier(2, [GameEvent.END_TURN])  {
                    Texture = TextureCache.Instance.GetItemTexture(ItemID.WrathPotion),
                }
            ]
        };
    }
}
