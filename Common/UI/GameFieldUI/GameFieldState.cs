using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;
using TerraTCG.Common.GameSystem.Drawing;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class GameFieldState : UIState
    {
        private GameFieldElement gameField;
        private HandElement handElement;
        internal ActionButtons actionButtons;

        public override void OnInitialize()
        {
            base.OnInitialize();
            gameField = new();
            SetRectangle(gameField, (Main.screenWidth - FieldRenderer.FIELD_WIDTH + 48) / 2, (Main.screenHeight - FieldRenderer.FIELD_HEIGHT + 16) / 2, 50, 50);
            Append(gameField);

            handElement = new();
            SetRectangle(handElement, Main.screenWidth / 2 - 186, Main.screenHeight - 180, 50, 50);
            Append(handElement);

            actionButtons = new();
            SetRectangle(actionButtons, Main.screenWidth / 2, Main.screenHeight / 2, 1, 1);
            Append(actionButtons);
        }
        private void SetRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
