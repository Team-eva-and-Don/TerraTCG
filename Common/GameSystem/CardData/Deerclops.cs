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
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class Deerclops : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "Deerclops",
            MaxHealth = 11,
			Points = 2,
            NPCID = NPCID.Deerclops,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.SNOW, CardSubtype.DEFENDER],
			DrawZoneNPC = CardOverlayRenderer.Instance.DrawDeerclopsNPC,
            FieldModifiers = () => [
                new FreezingModifier(1, [GameEvent.START_TURN], removeOnMyTurn: true),
            ],
			Modifiers = () => [
				new IceElemental.AttackCostModifier(),
			],
            Attacks = [
                new() {
                    Damage = -1,
                    Cost = 3,
                }
            ]
        };
    }
}
