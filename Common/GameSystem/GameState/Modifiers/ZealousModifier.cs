using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState.Modifiers
{
    internal class ZealousModifier : ICardModifier
    {
        public Asset<Texture2D> Texture { get; set; }

        public string Description => "";

        public void ModifyCardEntrance(Zone sourceZone) 
        {
            sourceZone.PlacedCard.IsExerted = false;
        }
    }
}
