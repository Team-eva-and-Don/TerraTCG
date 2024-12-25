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
    internal class Tim: ModSystem, ICardTemplate
    {
        private class MagicMissileModifier : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                var itemCount = sourceZone.Owner.Game.Turns
                    .Where(t => t.ActivePlayer == sourceZone.Owner)
                    .Select(t => t.UsedItemCount)
                    .Sum();
                attack.Damage = itemCount;
            }
        }

        public Card CreateCard() => new ()
        {
            Name = "Tim",
            MaxHealth = 8,
            MoveCost = 2,
            NPCID = NPCID.Tim,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.CASTER],
            Modifiers = [new MagicMissileModifier()],
            Attacks = [
                new() {
                    Name = "Magic Missile",
                    Damage = -1,
                    Cost = 3,
                }
            ]
        };
    }
}
