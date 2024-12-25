using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
    internal class GiantTortoise: ModSystem, ICardTemplate
    {
        private class TortoiseMustAttackModifier : ICardModifier
        {
            private Zone turtleZone;
            public void ModifyIncomingZoneSelection(Zone sourceZone, Zone endZone, ref List<Zone> destZones)
            {
                // Only allow attacks against the turtle
                if (!(turtleZone?.IsEmpty() ?? false))
                {
                    destZones = [turtleZone];
                }
            }

            public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                attack.SelfDamage += 1;
            }

            public bool ShouldRemove(GameEventInfo gameEvent) 
            {
                // TODO setting state in a modifier is not preferred
                turtleZone ??= gameEvent.Zone;
                return false;
            }
        }

        public Card CreateCard() => new ()
        {
            Name = "GiantTortoise",
            MaxHealth = 12,
            MoveCost = 3,
            CardType = CardType.CREATURE,
            NPCID = NPCID.GiantTortoise,
            SubTypes = [CardSubtype.EXPERT, CardSubtype.JUNGLE, CardSubtype.DEFENDER],
            Modifiers = [
                new TortoiseMustAttackModifier()
            ],
            Attacks = [
                new() {
                    Damage = 5,
                    Cost = 4,
                }
            ],
        };
    }
}
