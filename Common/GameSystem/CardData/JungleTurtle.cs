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
    internal class JungleTurtle : ModSystem, ICardTemplate
    {
        private class TurtleMustAttackModifier(Zone turtleZone) : ICardModifier
        {
            public void ModifyIncomingZoneSelection(Zone sourceZone, Zone endZone, ref List<Zone> destZones)
            {
                // Only allow attacks against the turtle
                if(!turtleZone.IsEmpty())
                {
                    destZones = [turtleZone];
                }
            }
            public bool ShouldRemove(GameEventInfo gameEvent) => 
                gameEvent.Event == GameEvent.END_TURN && !gameEvent.IsMyTurn;
        }

        public Card CreateCard() => new ()
        {
            Name = "JungleTurtle",
            MaxHealth = 9,
            MoveCost = 3,
            CardType = CardType.CREATURE,
            NPCID = NPCID.TurtleJungle,
            SubTypes = [CardSubtype.JUNGLE, CardSubtype.DEFENDER],
            Attacks = [
                new() {
                    Damage = 2,
                    Cost = 3,
                }
            ],
            Skills = [
                new() {
                    Cost = 2,
                    DoSkill = (GamePlayer player, Zone cardZone, Zone endZone) => {
                        player.Field.CardModifiers.Add(new TurtleMustAttackModifier(cardZone));
                    }
                }
            ]
        };
    }
}
