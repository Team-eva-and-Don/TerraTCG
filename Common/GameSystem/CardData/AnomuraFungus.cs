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
    internal class AnomuraFungus : BaseCardTemplate, ICardTemplate
    {
        // Creatures that attack the target gain lifesteal this turn
        private class LifestealOnAttackModifier: ICardModifier
        {
            public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                attack.SelfDamage -= 1;
            }
            public bool ShouldRemove(GameEventInfo eventInfo) => eventInfo.Event == GameEvent.END_TURN;
        }

        public override Card CreateCard() => new ()
        {
            Name = "AnomuraFungus",
            MaxHealth = 10,
            MoveCost = 2,
            NPCID = NPCID.AnomuraFungus,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.EXPERT, CardSubtype.MUSHROOM, CardSubtype.FIGHTER],
            Modifiers = () => [
                new LifestealModifier(1),
            ],
            Attacks = [
                new() {
                    Name = "Infect",
                    Damage = 4,
                    Cost = 3,
                    TargetModifiers = t=>[new LifestealOnAttackModifier()]
                }
            ]
        };
    }
}
