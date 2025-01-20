using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class Nurse : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "Nurse",
            CardType = CardType.TOWNSFOLK,
            SubTypes = [CardSubtype.TOWNSFOLK],
			Role = ZoneRole.DEFENSE,
            SelectInHandAction = (card, player) => new HealAndRemoveDebuffsAction(card, player),
        };
    }
}
