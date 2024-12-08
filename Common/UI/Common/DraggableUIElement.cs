using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerraTCG.Common.UI.Common
{
    internal class DraggableUIElement : CustomClickUIElement
    {
        internal Vector2 dragOffsetVector;
        internal bool IsDragging
        {
            get => dragOffsetVector != default;
            set => dragOffsetVector = default;
        }

        public override void Update(GameTime gameTime)
        {
            if(ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if(!IsDragging && Main.mouseLeft && ContainsPoint(Main.MouseScreen))
            {
                dragOffsetVector = Position - Main.MouseScreen;
            } else if (IsDragging && Main.mouseLeftRelease)
            {
                IsDragging = false;
            } else if (IsDragging)
            {
                Position = Main.MouseScreen + dragOffsetVector;
            }
        }
    }
}
