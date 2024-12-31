using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;
using TerraTCG.Common.UI.GameFieldUI;

namespace TerraTCG.Common.UI.PackOpeningUI
{
    internal class PackOpeningState : UIState
    {
        private PackOpeningAnimation openingAnimation;

        public TimeSpan StartTime { get => openingAnimation.StartTime;
            set { 
                openingAnimation.StartTime = value;
                openingAnimation.Reset();
            } 
        }

        public override void OnInitialize()
        {
            openingAnimation = new PackOpeningAnimation();
            Append(openingAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            GameFieldState.SetRectangle(openingAnimation, Main.screenWidth / 2, Main.screenHeight / 2, 1, 1);
        }

    }
}
