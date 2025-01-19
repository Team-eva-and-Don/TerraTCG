using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class Leech: BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "Leech",
            MaxHealth = 6,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.LeechHead,
			Priority = 10,
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawBestiaryZoneNPC,
            SubTypes = [CardSubtype.EVIL, CardSubtype.SCOUT],
            Attacks = [
                new() {
                    Damage = 3,
                    Cost = 2,
                }
            ],
        };
    }
}
