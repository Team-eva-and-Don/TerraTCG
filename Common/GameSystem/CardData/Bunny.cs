using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class Bunny : ModSystem, ICardTemplate
    {
        internal static ICardTemplate Instance => ModContent.GetInstance<Bunny>();
        public Card CreateCard() => new ()
        {
            Name = "Bunny",
            MaxHealth = 4,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Bunny,
            SubTypes = [CardSubtype.FOREST, CardSubtype.CRITTER],
            Role = ZoneRole.DEFENSE,
            Attacks = [
                new() {
                    Name = "Bunny Kick",
                    Damage = 1,
                    Cost = 1,
                }
            ],
            Skills = [
                new() {
                    Name = "Skill: Forest Wish",
                    Cost = 0,
                    Description = "Gain 1 MP",
                    DoSkill = (GamePlayer player, Zone zone) => {
                        player.Resources = player.Resources.UseResource(mana: -1);
                    }
                }
            ]
        };
    }
}
