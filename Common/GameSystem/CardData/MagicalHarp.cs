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
    internal class MagicalHarp: BaseCardTemplate, ICardTemplate
    {
		internal class BuffBiomeAndRoleModifier(CardSubtype biome, CardSubtype role, string cardName) : ICardModifier
		{

			public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
			{
				if(sourceZone.PlacedCard?.Template is Card card && card.SortType == biome && card.RoleSubtype == role)
				{
					attack.Damage += 1;
				}
			}

			public bool ShouldRemove(GameEventInfo eventInfo) => FieldModifierHelper.ShouldRemove(eventInfo, cardName);
		}

        public override Card CreateCard() => new ()
        {
            Name = "MagicalHarp",
            CardType = CardType.ITEM,
            SubTypes = [CardSubtype.EQUIPMENT, CardSubtype.ITEM],
            SelectInHandAction = (card, player) => new ApplySkillAction(card, player),
			ShouldTarget = zone => zone.PlacedCard is PlacedCard card && card.CurrentHealth <= card.Template.MaxHealth - 3,
            Skills = [ // TODO this is wonky, but item texts are drawn using the skill template
                new() { 
                    Cost = 2,
                    Texture = TextureCache.Instance.GetItemTexture(ItemID.MagicalHarp),
                    DoSkill = (GamePlayer player, Zone zone, Zone targetZone) => {
						var card = targetZone.PlacedCard;
						card.FieldModifiers.Add(
							new BuffBiomeAndRoleModifier(
								card.Template.SortType, 
								card.Template.SubTypes.Last(),
								card.Template.Name
							));
						// Need to apply to the field for this turn since that isn't
						// done automatically upon application
						targetZone.Owner.Field.CardModifiers.Add(card.FieldModifiers.Last());
					}
                }
            ],
        };
    }
}
