using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState.GameActions;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal enum CardType
    {
        CREATURE,
        ITEM,
        TOWNSFOLK
    }

    internal enum CardSubtype
    {
        FOREST,
        UNDEAD,
        CRITTER,
        EYE
    }

    internal class Card
    {
        internal string Name { get; set; }

        internal int NPCID { get; set; }

        internal Asset<Texture2D> Texture => ModContent.GetInstance<TerraTCG>().Assets.Request<Texture2D>($"Assets/Cards/{Name}");

        internal CardType CardType { get; set; }

        internal List<CardSubtype> SubTypes { get; set; }

        internal int MaxHealth { get; set; }

        internal int MoveCost { get; set; }

        internal List<IGameAction> InHandActions { get; set; }
        internal List<IGameAction> OnFieldActions { get; set; }

        internal List<Skill> Skills { get; set; }

        internal List<Attack> Attacks { get; set; }

        internal List<StaticModifier> Modifiers { get; set; }

        internal string CardName => Language.GetTextValue($"Mods.TerraTCG.Cards.{Name}.Name");

        internal string AttackName => Language.GetTextValue($"Mods.TerraTCG.Cards.{Name}.Attack.Name");
        internal string AttackDescription => Language.GetTextValue($"Mods.TerraTCG.Cards.{Name}.Attack.Description");
        internal string SkillName => Language.GetTextValue($"Mods.TerraTCG.Cards.{Name}.Skill.Name");
        internal string SkillDescription => Language.GetTextValue($"Mods.TerraTCG.Cards.{Name}.Skill.Description");

        internal string ModifierName => Language.GetTextValue($"Mods.TerraTCG.Cards.{Name}.Modifier.Name");
        internal string ModifierDescription => Language.GetTextValue($"Mods.TerraTCG.Cards.{Name}.Modifier.Description");

        internal bool HasAttack => (Attacks?.Count ?? 0) > 0;
        internal bool HasAttackDescription => HasAttack && Attacks[0].Description != null;
        internal bool HasSkill => (Skills?.Count ?? 0) > 0;
        internal bool HasSkillDescription => HasSkill && Skills[0].Description != null;
        internal bool HasModifier => (Modifiers?.Count ?? 0) > 0;

        internal string TypeLine => string.Join(" ", 
            SubTypes.Select(t => Language.GetTextValue($"Mods.TerraTCG.Cards.Types.{t}")));
            
    }
}
