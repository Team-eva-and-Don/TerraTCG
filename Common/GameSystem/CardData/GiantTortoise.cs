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
    internal class GiantTortoise: BaseCardTemplate, ICardTemplate
    {
        private class TortoiseMustAttackModifier : ICardModifier
        {
            private Zone turtleZone;
            public void ModifyIncomingZoneSelection(Zone sourceZone, Zone endZone, ref List<Zone> destZones)
            {
                // Don't allow attacks against the most damaged enemies
                if (!(turtleZone?.IsEmpty() ?? false))
                {
                    var lowestHPNonTurtle = endZone.Siblings
                        .Where(z => z.HasPlacedCard() && z != turtleZone)
                        .Select(z => z.PlacedCard.CurrentHealth)
						.DefaultIfEmpty()
                        .Min();

                    var newDestZones = destZones
                        .Where(z => z == turtleZone || (z.PlacedCard?.CurrentHealth ?? 0) > lowestHPNonTurtle)
                        .ToList();
                    if(newDestZones.Count > 0)
                    {
                        destZones = newDestZones;
                    }
                }
            }

            public bool ShouldRemove(GameEventInfo gameEvent) 
            {
                // TODO setting state in a modifier is not preferred
                turtleZone ??= gameEvent.Zone;
                return false;
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "GiantTortoise",
            MaxHealth = 10,
            MoveCost = 3,
            CardType = CardType.CREATURE,
            NPCID = NPCID.GiantTortoise,
            SubTypes = [CardSubtype.EXPERT, CardSubtype.JUNGLE, CardSubtype.DEFENDER],
            Modifiers = () => [
                new TortoiseMustAttackModifier(),
                new SpikedModifier(2),
            ],
            Attacks = [
                new() {
                    Damage = 5,
                    Cost = 5,
                }
            ],
        };
    }
}
