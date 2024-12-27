using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class ToxicSludge : ModSystem, ICardTemplate
    {
        private void SludgeAttack(Attack attack, Zone sourceZone, Zone targetZone)
        {
            var latestEquipment = targetZone.PlacedCard?.CardModifiers
                .Where(m => m.Source == CardSubtype.EQUIPMENT)
                .LastOrDefault();

            if(latestEquipment != null)
            {
                targetZone.PlacedCard.CardModifiers.Remove(latestEquipment);
                // TODO sequence this after damage-taking animation
                // targetZone.QueueAnimation(new RemoveModifierAnimation(targetZone.PlacedCard, latestEquipment.Texture));
            }
            Attack.DefaultAttack(attack, sourceZone, targetZone);
        }

        public Card CreateCard() => new ()
        {
            Name = "ToxicSludge",
            MaxHealth = 7,
            MoveCost = 2,
            NPCID = NPCID.ToxicSludge,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.CASTER],
            Attacks = [
                new() {
                    Name = "Corrode",
                    Damage = 2,
                    Cost = 2,
                    DoAttack = SludgeAttack,
                }
            ]
        };
    }
}
