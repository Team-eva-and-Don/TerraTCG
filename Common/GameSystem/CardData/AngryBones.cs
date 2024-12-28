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
    internal class AngryBones : BaseCardTemplate, ICardTemplate
    {
        internal class EquipCostModifier : ICardModifier
        {
            public void ModifyIncomingSkill(ref Skill skill, Card sourceCard) 
            {
                if(sourceCard.SubTypes.Contains(CardSubtype.EQUIPMENT))
                {
                    skill.Cost = Math.Max(1, skill.Cost - 1);
                }
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "AngryBones",
            MaxHealth = 8,
            MoveCost = 2,
            NPCID = NPCID.AngryBones,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.FIGHTER],
            Modifiers = [
                new EquipCostModifier()
            ],
            Attacks = [
                new() {
                    Name = "Cursed Axe",
                    Damage = 3,
                    Cost = 3,
                }
            ]
        };
    }
}
