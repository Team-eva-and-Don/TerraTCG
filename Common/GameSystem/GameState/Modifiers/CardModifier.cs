using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace TerraTCG.Common.GameSystem.GameState.Modifiers
{
    internal enum GameEvent
    {
        END_TURN
    }

    internal interface ICardModifier
    {
        public Asset<Texture2D> Texture { get; }
        public string Description { get; }
        public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
        {
            // no-op
        }

        public void ModifyZoneSelection(Zone sourceZone, List<Zone> destZones)
        {
            // no-op
        }

        public bool ShouldRemove(GameEvent gameEvent) {
            return false;
        }
    }
}
