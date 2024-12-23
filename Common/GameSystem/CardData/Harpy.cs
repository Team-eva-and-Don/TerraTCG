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
    internal class Harpy : ModSystem, ICardTemplate
    {
        private class HarpyDefenseBreakModifier(Zone targetZone) : ICardModifier
        {
            public void ModifyIncomingZoneSelection(Zone sourceZone, Zone endZone, ref List<Zone> destZones)
            {
                if(!destZones.Contains(targetZone))
                {
                    destZones.Add(targetZone);
                }
            }
            public bool ShouldRemove(GamePlayer turnPlayer, GameEvent gameEvent) => gameEvent == GameEvent.END_TURN;
        }

        public Card CreateCard() => new ()
        {
            Name = "Harpy",
            MaxHealth = 7,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Harpy,
            SubTypes = [CardSubtype.SKY, CardSubtype.SCOUT],
            Attacks = [
                new() {
                    Damage = 2,
                    Cost = 2,
                    DoAttack = (Attack attack, Zone sourceZone, Zone destZone) => {
                        Attack.DefaultAttack(attack, sourceZone, destZone);
                        destZone.PlacedCard.CardModifiers.Add(new HarpyDefenseBreakModifier(destZone));
                    }
                }
            ],
            Modifiers = [
                new EvasiveModifier(),
            ]
        };
    }
}
