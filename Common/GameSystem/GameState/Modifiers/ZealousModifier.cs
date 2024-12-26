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
        public int Amount => 1;

        public Asset<Texture2D> Texture { get; set; }

        public string Description => "";

        public void ModifyCardEntrance(Zone sourceZone) 
        {
            // Having cards enter unpaused on turn 1 screws up a 
            // bunch of enemy decision-making.
            if(sourceZone.Owner.Game.CurrentTurn.TurnCount > 1)
            {
                sourceZone.PlacedCard.IsExerted = false;
            }
        }
    }
}
