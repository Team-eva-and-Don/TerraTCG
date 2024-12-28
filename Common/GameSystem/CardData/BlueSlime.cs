using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class BlueSlime : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "BlueSlime",
            MaxHealth = 6,
            MoveCost = 2,
            NPCID = NPCID.BlueSlime,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.FOREST, CardSubtype.SLIME, CardSubtype.FIGHTER],
            DrawZoneNPC = CardOverlayRenderer.Instance.DrawSlimeNPC(1f, new Color(0, 80, 255, 180)),
            Attacks = [
                new() {
                    Name = "Slimed",
                    Damage = 3,
                    Cost = 2,
                }
            ]
        };
    }
}
