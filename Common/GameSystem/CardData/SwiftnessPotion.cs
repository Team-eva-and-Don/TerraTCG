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
    internal class SwiftnessPotion: BaseCardTemplate, ICardTemplate
    {
        public static bool ShouldUseUnpauseItem(Zone zone) =>
            zone.PlacedCard is PlacedCard card && card.IsExerted &&
            card.GetAttackWithModifiers(zone, null).Cost < zone.Owner.Resources.Mana;
        public override Card CreateCard() => new ()
        {
            Name = "SwiftnessPotion",
            CardType = CardType.ITEM,
            SubTypes = [CardSubtype.CONSUMABLE, CardSubtype.ITEM],
            SelectInHandAction = (card, player) => new ApplySkillAction(card, player),
            Role = ZoneRole.OFFENSE,
            ShouldTarget = ShouldUseUnpauseItem,
            Skills = [ // TODO this is wonky, but item texts are drawn using the skill template
                new() { 
                    Cost = 1,
                    Texture = TextureCache.Instance.GetItemTexture(ItemID.SwiftnessPotion),
                    DoSkill = (GamePlayer player, Zone zone, Zone targetZone) => targetZone.PlacedCard.IsExerted = false,
                }
            ],
        };
    }
}
