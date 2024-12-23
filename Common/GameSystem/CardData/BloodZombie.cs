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
    internal class BloodZombie : ModSystem, ICardTemplate
    {
        public Card CreateCard() => new ()
        {
            Name = "BloodZombie",
            MaxHealth = 6,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.BloodZombie,
            SubTypes = [CardSubtype.BLOOD_MOON, CardSubtype.SCOUT],
            Attacks = [
                new() {
                    Damage = 5,
                    Cost = 3,
                    DoAttack = (Attack attack, Zone sourceZone, Zone destZone) => {
                        Attack.DefaultAttack(attack, sourceZone, destZone);
                        sourceZone.PlacedCard.CurrentHealth -= 1;
                    }
                }
            ],
            Modifiers = [
                new ZealousModifier(),
            ]
        };
    }
}
