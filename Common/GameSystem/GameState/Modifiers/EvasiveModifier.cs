using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState.Modifiers
{
    internal class EvasiveModifier : ICardModifier
    {
        public Asset<Texture2D> Texture { get; set; }
        public void ModifyZoneSelection(Zone sourceZone, Zone endZone, ref List<Zone> destZones)
        {
            // allow the targeting of blocked enemy zones
            destZones = endZone.Owner.Field.Zones.Where(z => !z.IsEmpty()).ToList();
        }
    }
}
