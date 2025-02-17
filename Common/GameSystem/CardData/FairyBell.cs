using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class FairyBell: BaseCardTemplate, ICardTemplate
    {
		internal static bool SharesBiomeAndRole(Card c1, Card c2) =>
			c1.RoleSubtype == c2.RoleSubtype && c1.SortType == c2.SortType;

        public override Card CreateCard() => new ()
        {
            Name = "FairyBell",
            CardType = CardType.ITEM,
            SubTypes = [CardSubtype.CONSUMABLE, CardSubtype.ITEM],
            SelectInHandAction = (card, player) => new ApplySkillAction(card, player),
			ShouldTarget = zone => zone.PlacedCard is PlacedCard card && card.CurrentHealth <= card.Template.MaxHealth - 3,
            Role = ZoneRole.DEFENSE,
            Skills = [ // TODO this is wonky, but item texts are drawn using the skill template
                new() { 
                    Cost = 1,
                    Texture = TextureCache.Instance.GetItemTexture(ItemID.FairyBell),
                    DoSkill = (GamePlayer player, Zone zone, Zone targetZone) => {
						targetZone.PlacedCard.Heal(1);
						foreach(var sibling in targetZone.Siblings) {
							if(sibling.PlacedCard is PlacedCard card && SharesBiomeAndRole(card.Template, targetZone.PlacedCard.Template)) {
								sibling.PlacedCard.Heal(1);
							}
						}
					}
                }
            ],
        };
    }
}
