using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerraTCG.Common.UI.Common
{
    class RadialButton : CustomClickUIElement
    {
        internal const int CLICK_RADIUS = 20;

        public bool ContainsMouse => (Main.MouseScreen - Position).LengthSquared() < CLICK_RADIUS * CLICK_RADIUS;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
