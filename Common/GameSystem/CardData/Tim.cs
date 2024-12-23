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
                attack.Cost = Math.Max(1, sourceZone.Owner.Resources.Mana);
                attack.Damage = sourceZone.Owner.Resources.Mana;
            }
        }

        public Card CreateCard() => new ()
        {
            Name = "Tim",
            MaxHealth = 7,
            MoveCost = 2,
            NPCID = NPCID.Tim,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.CASTER],
            Modifiers = [new MagicMissileModifier()],
            Attacks = [
                new() {
                    Name = "Magic Missile",
                    Damage = -1,
                    Cost = -1,
                }
            ]
        };
    }
}
