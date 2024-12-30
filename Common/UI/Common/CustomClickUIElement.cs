using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;

namespace TerraTCG.Common.UI.Common
{
    internal class CustomClickUIElement : UIElement
    {
        internal Vector2 Position
        {
            get => new(Left.Pixels, Top.Pixels);
            set
            {
                Left.Pixels = value.X;
                Top.Pixels = value.Y;
            }


        } 

        private bool clickStarted;

        // Helper method to return true if it's evaluated both while the mouse is down
        // inside the element, then evaluated again while the mouse is up in the element
        internal virtual bool IsClicked()
        {
            if(Main.mouseLeft)
            {
                clickStarted = true;
                return false;
            } else if (Main.mouseLeftRelease && clickStarted)
            {
                clickStarted = false;
                return true;
            } else
            {
                clickStarted = false;
                return false;
            }
        }

        internal static void PlayTickIfMouseEntered(Rectangle bounds)
        {
            var didEnter = bounds.Contains(Main.mouseX, Main.mouseY) && !bounds.Contains(Main.lastMouseX, Main.lastMouseY);

            if(didEnter)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }

        internal static void PlayTickIfMouseEntered(Vector2 point, float radius)
        {
            var didEnter = Vector2.DistanceSquared(point, Main.MouseScreen) <= radius * radius &&
                Vector2.DistanceSquared(point, new Vector2(Main.lastMouseX, Main.lastMouseY)) >= radius * radius;

            if(didEnter)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
    }
}
