using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class KingSlime : BaseCardTemplate, ICardTemplate
    {
        private class KingSlimeDamageBoost : ICardModifier
        {
            public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
            {
                if(sourceZone.PlacedCard?.Template.SubTypes.Contains(CardSubtype.SLIME) ?? false)
                {
                    attack.Damage += 1;
                }
            }
            public bool ShouldRemove(GameEventInfo eventInfo) => eventInfo.Event == GameEvent.END_TURN; 
        }

        public override Card CreateCard() => new ()
        {
            Name = "KingSlime",
            MaxHealth = 10,
            MoveCost = 2,
            Points = 2,
            NPCID = NPCID.KingSlime,
            CardType = CardType.CREATURE,
            Role = ZoneRole.DEFENSE,
            SubTypes = [CardSubtype.BOSS, CardSubtype.FOREST, CardSubtype.SLIME, CardSubtype.FIGHTER],
            DrawZoneNPC = CardOverlayRenderer.Instance.DrawKingSlimeNPC,
            Skills = [
                new() {
                    Name = "Skill: Royal Decree",
                    Cost = 1,
                    DoSkill = (GamePlayer player, Zone zone, Zone targetZone) => {
                        player.Field.CardModifiers.Add(new KingSlimeDamageBoost());
                        foreach(var slimeZone in player.Field.Zones
                            .Where(z=>z.PlacedCard?.Template.SubTypes.Contains(CardSubtype.SLIME) ?? false)) {
                            slimeZone.PlacedCard.Heal(1);
                        }
                    }
                }
            ],
            Attacks = [
                new() {
                    Damage = 4,
                    Cost = 4,
                }
            ]
        };
    }
}
