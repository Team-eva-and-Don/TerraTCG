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
    internal class GoblinScout : BaseCardTemplate, ICardTemplate
    {
        internal class DamageModifier : ICardModifier
        {
			public Asset<Texture2D> Texture { get; set; }
			public Card SourceCard { get; set; }

            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
				if(sourceZone.Owner != sourceZone.Owner.Game.CurrentTurn.ActivePlayer)
				{
					return;
				}
                var itemCount = sourceZone.Owner.Game.CurrentTurn.UsedItemCount;
                attack.Damage += itemCount;
            }
        }

        public override Card CreateCard() => new ()
        {
            Name = "GoblinScout",
            MaxHealth = 7,
            MoveCost = 1,
            NPCID = NPCID.GoblinScout,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.GOBLIN_ARMY, CardSubtype.SCOUT],
            Modifiers = () => [
                new ZealousModifier(),
                new DamageModifier(),
            ],
            Attacks = [
                new() {
                    Name = "Scout",
                    Damage = 2,
                    Cost = 2,
                }
            ]
        };
    }
}
