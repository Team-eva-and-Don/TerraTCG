using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;

namespace TerraTCG.Common.UI.Common
{
    internal class CustomClickUIElement : UIElement
    {
        private bool clickStarted;

        // Helper method to return true if it's evaluated both while the mouse is down
        // inside the element, then evaluated again while the mouse is up in the element
        internal bool IsClicked()
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
    }
}
