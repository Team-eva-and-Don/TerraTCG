using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class ArmoredSkeleton : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "ArmoredSkeleton",
            MaxHealth = 10,
            MoveCost = 2,
            NPCID = NPCID.ArmoredSkeleton,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.EXPERT, CardSubtype.CAVERN, CardSubtype.FIGHTER],
            Modifiers = () => [
                new UndeadMiner.EquipCostModifier(),
                new AngryBones.EquipCostModifier(),
            ],
            Attacks = [
                new() {
                    Name = "Mighty Swing",
                    Damage = 4,
                    Cost = 4,
                }
            ]
        };
    }
}
