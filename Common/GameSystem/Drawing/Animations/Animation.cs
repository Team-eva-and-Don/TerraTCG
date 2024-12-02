using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.Drawing.Animations
{
    internal interface IAnimation
    {
        // Draw the card within the zone itself, if applicable
        void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation);

        // Draw additional items (such as an NPC sprite) on top of the zone
        void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale);

        bool IsComplete();
    }
}
