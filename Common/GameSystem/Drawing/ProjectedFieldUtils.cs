using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.Drawing
{
    // Yet another utility class for a 2-tuple of floats
    internal readonly struct ProjBounds(float min, float max)
    {
        public float Min { get; } = min;
        public float Max { get; } = max;

        public float Span => Max - Min;

        public float Lerp(float percent) => MathHelper.Lerp(Min, Max, percent);

    }

    // Set of utilities for drawing UI elements on top of the perspective-rendered game field
    // Converts between 3 2D coordinate systems:
    // "Texture space", the (0 -> TEXTURE_WIDTH/HEIGHT) coordinates used to draw zones
    // "World space", the (-1 -> 1) coordinates of the quads at z=0 that comprise the 3d
    // primitives rendered with a perspective
    // "Screen space", the (0 -> TEXTURE_WIDTH/HEIGHT) position of the mouse on the user's screen
    internal class ProjectedFieldUtils : ModSystem
    {
        public static ProjectedFieldUtils Instance => ModContent.GetInstance<ProjectedFieldUtils>();

        private Viewport Viewport { get; set; }

        public override void Load()
        {
            Viewport = new(0, 0, FieldRenderer.FIELD_WIDTH, FieldRenderer.FIELD_HEIGHT);
        }

        public ProjBounds GetTextureSpaceXBoundsForZone(GamePlayer player, Zone zone)
        {
            // need to account for mirroring of opponent's rows
            var zoneCount = player.Field.Zones.Count;
            int horizontalSlot;
            int baseOffset = FieldRenderer.CARD_WIDTH + FieldRenderer.CARD_MARGIN + FieldRenderer.FIELD_MARGIN;
            if(player.Owns(zone))
            {
                horizontalSlot = player.Field.Zones.IndexOf(zone) % (zoneCount / 2);
            } else
            {
                horizontalSlot = 2 - player.Opponent.Field.Zones.IndexOf(zone) % (zoneCount / 2);
            }
            float xMin = baseOffset + (FieldRenderer.CARD_WIDTH + FieldRenderer.CARD_MARGIN) * horizontalSlot;
            float xMax = xMin + FieldRenderer.CARD_WIDTH;
            return new ProjBounds(xMin, xMax);
        }

        private ProjBounds GetTextureSpaceYBoundsForZone(GamePlayer player, Zone zone)
        {
            var zoneIdx = player.Field.Zones.IndexOf(zone);
            var oppZoneIdx = player.Opponent.Field.Zones.IndexOf(zone);
            var zonesPerRow = player.Field.Zones.Count / 2;

            float y0;

            // locations in texture space
            if(zoneIdx >= 0 && zoneIdx < zonesPerRow)
            {
                y0 = FieldRenderer.FIELD_HEIGHT - FieldRenderer.CARD_MARGIN - 2 * FieldRenderer.CARD_HEIGHT - FieldRenderer.FIELD_MARGIN;
            } else if(zoneIdx >= 0 && zoneIdx >= zonesPerRow)
            {
                y0 = FieldRenderer.FIELD_HEIGHT - FieldRenderer.CARD_HEIGHT - FieldRenderer.FIELD_MARGIN;
            } else if(oppZoneIdx >= 0 && oppZoneIdx < zonesPerRow)
            {
                y0 = FieldRenderer.CARD_HEIGHT + FieldRenderer.CARD_MARGIN + FieldRenderer.FIELD_MARGIN;
            } else
            {
                y0 = FieldRenderer.FIELD_MARGIN;
            }
            float y1 = y0 + FieldRenderer.CARD_HEIGHT;
            return new ProjBounds(y0, y1);
        }

        // Convert a (1 -> -1) space vector to a (0 -> FIELD_SIZE) space vector
        private Vector2 FloatBoundsToTextureBounds(Vector2 floatBounds)
        {
            return new Vector2(
                FieldRenderer.FIELD_WIDTH / 2f * (1 + floatBounds.X),
                FieldRenderer.FIELD_HEIGHT / 2f * (1 - floatBounds.Y)
            );
        }

        // Convert a  (0 -> FIELD_SIZE) space vector to a(1 -> -1) space vector
        private Vector2 TextureBoundsToFloatBounds(Vector2 textureBounds)
        {
            return new Vector2(
                -1 + 2f * textureBounds.X / FieldRenderer.FIELD_WIDTH,
                1 - 2f * textureBounds.Y / FieldRenderer.FIELD_HEIGHT
            );
        }



        // Get the X and Y bounds for a zone, in texture space coordinates
        public Rectangle GetBoundsForZone(GamePlayer player, Zone zone)
        {
            var yBounds = GetTextureSpaceYBoundsForZone(player, zone);
            var xBounds = GetTextureSpaceXBoundsForZone(player, zone);

            return new((int)xBounds.Min, (int)yBounds.Min, (int)xBounds.Span, (int)yBounds.Span);
        }

        // Project a mouse position in screen coordinates onto world coordinates
        public Vector2 ScreenSpaceToWorldSpace(Vector2 screenVector)
        {
            var field = FieldRenderer.Instance;
            var nearPoint = Viewport.Unproject(new Vector3(screenVector, 0), field.projection, field.view, field.world);
            var farPoint = Viewport.Unproject(new Vector3(screenVector, 1), field.projection, field.view, field.world);

            var mouseRay = new Ray(nearPoint, Vector3.Normalize(farPoint - nearPoint));

            var fieldPlane = new Plane(new(-1, 1, 0), new(1, 1, 0), new(-1, -1, 0));

            var result = mouseRay.Intersects(fieldPlane);
            if(result is float intersectVal)
            {
                var worldPoint = mouseRay.Position + mouseRay.Direction * intersectVal;

                return FloatBoundsToTextureBounds(new Vector2(worldPoint.X, worldPoint.Y));
            }
            return default;
        }

        // Return whether the mouse position, projected onto world coordinates,
        // falls within the specified zone
        public bool ZoneContainsScreenVector(GamePlayer player, Zone zone, Vector2 screenVector)
        {
            // via https://gamedev.stackexchange.com/a/26116
            var worldPoint = ScreenSpaceToWorldSpace(screenVector);
            var bounds = GetBoundsForZone(player, zone);

            return bounds.Contains((int)worldPoint.X, (int)worldPoint.Y);
        }

        // Given a zone and a point within that zone, return the screen
        // location where that point in world coordinates appears
        public Vector2 WorldSpaceToScreenSpace(GamePlayer player, Zone zone, Vector2 lerpPoint)
        {
            //normalize to (-1, 1)
            var xPos = GetTextureSpaceXBoundsForZone(player, zone).Lerp(lerpPoint.X);
            var yPos = GetTextureSpaceYBoundsForZone(player, zone).Lerp(lerpPoint.Y);

            var textureVector = new Vector2(xPos, yPos);
            var worldVector = new Vector3(TextureBoundsToFloatBounds(textureVector), 0f);
            var field = FieldRenderer.Instance;
            var screenPos = Viewport.Project(worldVector, field.projection, field.view, field.world);

            return new(screenPos.X, screenPos.Y);
        }

        // Approximate the draw scale for a zone by measuring the width of the center of
        // the zone in screen space, compared to the full width of a zone
        public float GetXScaleForZone(GamePlayer player, Zone zone, float yLerpPoint = 0.5f)
        {
            var xBounds = GetTextureSpaceXBoundsForZone(player, zone);
            var yPos = GetTextureSpaceYBoundsForZone(player, zone).Lerp(yLerpPoint);

            var xVector0 = TextureBoundsToFloatBounds(new(xBounds.Min, yPos));
            var xVector1 = TextureBoundsToFloatBounds(new(xBounds.Max, yPos));

            var worldVector0 = new Vector3(xVector0, 0f);
            var worldVector1 = new Vector3(xVector1, 0f);
            var field = FieldRenderer.Instance;
            var screenPos0 = Viewport.Project(worldVector0, field.projection, field.view, field.world);
            var screenPos1 = Viewport.Project(worldVector1, field.projection, field.view, field.world);

            float screenDistance = (screenPos0 - screenPos1).Length();
            return screenDistance / FieldRenderer.CARD_WIDTH;
        }
    }
}
