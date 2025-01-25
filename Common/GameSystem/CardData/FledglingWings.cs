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
    internal class FledglingWings : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "FledglingWings",
            CardType = CardType.ITEM,
            SubTypes = [CardSubtype.EQUIPMENT, CardSubtype.ITEM],
            SelectInHandAction = (card, player) => new ApplyModifierAction(card, player),
            Skills = [ // TODO this is wonky, but item texts are drawn using the skill template
                new() { Cost = 2 }
            ],
            // TODO probably want a flag for this instead of type introspection
            ShouldTarget = (Zone zone) => !(zone.PlacedCard?.CardModifiers.Where(m=> m is EvasiveModifier).Any() ?? false),
            Modifiers = () => [
                new EvasiveModifier() { 
                    Texture = TextureCache.Instance.GetItemTexture(4978),
                }
                
            ]
        };
    }
}
