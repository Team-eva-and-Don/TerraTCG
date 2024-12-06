using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class Dryad : ModSystem, ICardTemplate
    {
        internal static ICardTemplate Instance => ModContent.GetInstance<Dryad>();
        public Card CreateCard() => new ()
        {
            Name = "Dryad",
            CardType = CardType.TOWNSFOLK,
            SubTypes = [CardSubtype.TOWNSFOLK],
            SelectInHandAction = (card, player) => new BounceCardAction(card, player),
            Modifiers = [
                new() {
                    // TODO this should probably not be empty, just used to set the flag
                    // to draw modifier text
                }
            ]
        };
    }
}
