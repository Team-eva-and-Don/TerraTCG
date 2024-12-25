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
    internal class Piranha : ModSystem, ICardTemplate
    {
        private class PiranhaBleedDamage: ICardModifier
        {
            public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                attack.Damage += 1;
            }
        }

        public Card CreateCard() => new ()
        {
            Name = "Piranha",
            MaxHealth = 6,
            MoveCost = 2,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Piranha,
            SubTypes = [CardSubtype.JUNGLE, CardSubtype.FIGHTER],
            Attacks = [
                new() {
                    Damage = 6,
                    Cost = 4,
                    TargetModifiers = z=>[new PiranhaBleedDamage()]
                }
            ],
        };
    }
}
