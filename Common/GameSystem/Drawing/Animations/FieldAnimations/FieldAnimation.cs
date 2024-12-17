using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations
{
    internal interface IFieldAnimation
    {
        TimeSpan StartTime { get; }

        // Draw the field
        void DrawFieldOverlay(SpriteBatch spriteBatch, Vector2 basePosition);

        bool IsDefault() => false;
        bool IsComplete();
    }
}
