using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        internal Asset<Texture2D> Texture => ModContent.GetInstance<TerraTCG>().Assets.Request<Texture2D>($"Assets/Cards/{Name}");

        internal CardType CardType { get; set; }

        internal List<CardSubtype> SubTypes { get; set; }

        internal int MaxHealth { get; set; }

        internal int MoveCost { get; set; }

        internal List<IGameAction> InHandActions { get; set; }
        internal List<IGameAction> OnFieldActions { get; set; }

        internal List<Ability> Abilities { get; set; }

        internal List<Attack> Attacks { get; set; }

    }
}
