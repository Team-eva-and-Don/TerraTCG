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
    internal class Shackle : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "Shackle",
            CardType = CardType.ITEM,
            SubTypes = [CardSubtype.EQUIPMENT, CardSubtype.ITEM],
            SelectInHandAction = (card, player) => new ApplyModifierAction(card, player),
            ShouldTarget = z => true,
            Skills = [ // TODO this is wonky, but item texts are drawn using the skill template
                new() { Cost = 1 }
            ],
            Modifiers = () => [
                new ReduceDamageModifier(2, [GameEvent.AFTER_RECEIVE_ATTACK])  {
                    Texture = TextureCache.Instance.GetItemTexture(ItemID.Shackle),
                }
            ]
        };
    }
}
