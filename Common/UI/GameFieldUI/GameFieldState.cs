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
        internal UseSkillButton actionButtons;
        internal PassTurnButton passTurnButton;
        internal CardPreviewElement previewElement;

        public override void OnInitialize()
        {
            base.OnInitialize();
            gameField = new();
            SetRectangle(gameField, (Main.screenWidth - FieldRenderer.FIELD_WIDTH + 48) / 2, (Main.screenHeight - FieldRenderer.FIELD_HEIGHT + 16) / 2);
            Append(gameField);

            handElement = new();
            SetRectangle(handElement, Main.screenWidth / 2, Main.screenHeight - HandElement.CARD_HEIGHT);
            Append(handElement);

            previewElement = new();
            SetRectangle(previewElement, gameField.Position.X - 120, gameField.Position.Y + 175, 180, 240);
            Append(previewElement);

            actionButtons = new();
            SetRectangle(actionButtons, Main.screenWidth / 2, Main.screenHeight / 2);
            Append(actionButtons);

            passTurnButton = new();
            SetRectangle(passTurnButton, (Main.screenWidth + FieldRenderer.FIELD_WIDTH) / 2, Main.screenHeight / 2);
            Append(passTurnButton);
        }
        private void SetRectangle(UIElement uiElement, float left, float top, float width = 1, float height = 1)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            SetRectangle(gameField, (Main.screenWidth - FieldRenderer.FIELD_WIDTH + 48) / 2, (Main.screenHeight - FieldRenderer.FIELD_HEIGHT) / 2, 1, 1);
            SetRectangle(handElement, Main.screenWidth / 2, Main.screenHeight - HandElement.CARD_HEIGHT, 1, 1);
            SetRectangle(passTurnButton, Main.screenWidth / 2 + 3 * FieldRenderer.FIELD_WIDTH / 8, Main.screenHeight / 2);
            base.Update(gameTime);
        }
    }
}
