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
    internal class Squirrel : ModSystem, ICardTemplate
    {
        public Card CreateCard() => new ()
        {
            Name = "Squirrel",
            MaxHealth = 5,
            MoveCost = 1,
            CardType = CardType.CREATURE,
            NPCID = NPCID.Squirrel,
            SubTypes = [CardSubtype.FOREST, CardSubtype.CRITTER],
            Role = ZoneRole.DEFENSE,
            Attacks = [
                new() {
                    Name = "Acorn Toss",
                    Damage = 1,
                    Cost = 1,
                }
            ],
            Skills = [
                new() {
                    Name = "Skill: Forest Cheer",
                    Cost = 1,
                    DoSkill = (GamePlayer player, Zone zone) => {
                        player.Field.CardModifiers.Add(new FlatDamageModifier(1, [GameEvent.AFTER_ATTACK, GameEvent.END_TURN]));
                    }
                }
            ]
        };
    }
}
