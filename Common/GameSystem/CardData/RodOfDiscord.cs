using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class RodOfDiscord : BaseCardTemplate, ICardTemplate
    {
        public override Card CreateCard() => new ()
        {
            Name = "RodOfDiscord",
            CardType = CardType.ITEM,
            SubTypes = [CardSubtype.EQUIPMENT, CardSubtype.ITEM],
            SelectInHandAction = (card, player) => new ApplySkillAction(card, player),
            Skills = [
                new() { 
                    Cost = 1,
                    Texture = TextureCache.Instance.GetItemTexture(ItemID.RodofDiscord),
                    DoSkill = (GamePlayer player, Zone zone, Zone targetZone) => 
						targetZone.PlacedCard.Skill = BotDecks.GetCard<ChaosElemental>().Skills[0],
                }
            ],
        };
    }
}
