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
    internal class SwiftnessPotion: ModSystem, ICardTemplate
    {
        public Card CreateCard() => new ()
        {
            Name = "SwiftnessPotion",
            CardType = CardType.ITEM,
            SubTypes = [CardSubtype.CONSUMABLE, CardSubtype.ITEM],
            SelectInHandAction = (card, player) => new ApplySkillAction(card, player),
            Role = ZoneRole.OFFENSE,
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
