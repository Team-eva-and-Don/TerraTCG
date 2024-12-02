using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.Drawing
{
    // Set of utilities for drawing UI elements on top of the perspective-rendered game field
    // Coordinate system is as follows
    //
    //           y=-FIELD_HEIGHT
    //            |
    //            |
    //            |
    //            |
    //            |
    // ------------------------
    // x=       (0,0)       x=
    //  -FIELD_WIDTH/2       FIELD_WIDTH/2
    //
    // TODO computing this properly outside of trial and error will be a nightmare
    internal readonly struct ProjBounds(float min, float max)
    {
        public float Min { get; } = min;
        public float Max { get; } = max;

        public float Center => (Min + Max) / 2;

        public float Lerp(float percent) => MathHelper.Lerp(Min, Max, percent);

        public static ProjBounds operator*(ProjBounds left, float right) =>
            new(right *  left.Min, right * left.Max);
    }
    internal class ProjectedFieldUtils : ModSystem
    {
        public static ProjectedFieldUtils Instance => ModContent.GetInstance<ProjectedFieldUtils>();

        internal float[] rowHeights = [
            -(475f / 576f), -(405f / 576f), 
            -(420f / 575f), -(320f / 567f),
            -(255f / 576f), -(155f / 576f),
            -(145f / 565f), -(30f  / 566f),
        ];

        internal float[] widthScaleFactors = [
            1f, // at Y = rowHeights[-1]
            65f / 90f, // at Y = rowHeights[0]
        ];


        public ProjBounds GetYBoundsForZone(GamePlayer player, Zone zone)
        {
            var zoneIdx = player.Field.Zones.IndexOf(zone);
            var oppZoneIdx = player.Opponent.Field.Zones.IndexOf(zone);
            var zonesPerRow = player.Field.Zones.Count / 2;
            var scaleFactor = FieldRenderer.FIELD_HEIGHT;
            if (zoneIdx >= 0 && zoneIdx < zonesPerRow)
            {
                return new ProjBounds(rowHeights[4], rowHeights[5]) * scaleFactor;
            } else if (zoneIdx >= zonesPerRow)
            {
                return new ProjBounds(rowHeights[6], rowHeights[7]) * scaleFactor;
            } else if (oppZoneIdx >= 0 && oppZoneIdx < zonesPerRow)
            {
                return new ProjBounds(rowHeights[2], rowHeights[3]) * scaleFactor;
            } else if (oppZoneIdx > zonesPerRow)
            {
                return new ProjBounds(rowHeights[0], rowHeights[2]) * scaleFactor;
            } else
            {
                // TODO raise exception here?
                return new ProjBounds();
            }
        }

        public float GetScaleFactorAt(float y) =>
            MathHelper.Lerp(
                widthScaleFactors[0], widthScaleFactors[1],
                (float)(y/FieldRenderer.FIELD_HEIGHT - rowHeights[7]) / (rowHeights[0] - rowHeights[7]));

        public ProjBounds GetXBoundsForZone(GamePlayer player, Zone zone)
        {
            // need to account for mirroring of opponent's rows
            var zoneCount = player.Field.Zones.Count;
            int horizontalSlot;
            if(player.Owns(zone))
            {
                horizontalSlot = player.Field.Zones.IndexOf(zone) % (zoneCount / 2) - 1;
            } else
            {
                horizontalSlot = 1 - player.Opponent.Field.Zones.IndexOf(zone) % (zoneCount / 2);
            }

            float xMin = (FieldRenderer.CARD_WIDTH + FieldRenderer.CARD_MARGIN) * horizontalSlot - FieldRenderer.CARD_WIDTH / 2;
            float xMax = (FieldRenderer.CARD_WIDTH + FieldRenderer.CARD_MARGIN) * (horizontalSlot + 1) - FieldRenderer.CARD_WIDTH / 2;

            return new ProjBounds(xMin, xMax);

        }
    }
}
