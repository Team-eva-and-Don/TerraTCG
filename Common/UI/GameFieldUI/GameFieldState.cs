using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class GameFieldState : UIState
    {
        internal GameFieldElement gameField;
        internal HandElement handElement;
        internal OpponentHandElement oppHandElement;
        internal UseSkillButton actionButtons;
        internal PassTurnButton passTurnButton;
        internal CardPreviewElement previewElement;
        internal CancelResumeGameButton cancelButton;
        internal ActionLogElement actionLog;

        // TODO "hiding" the field by sliding it off the bottom of the screen is a bit iffy
        internal static int UI_MAX_HEIGHT => Main.screenHeight - (Main.screenHeight - FieldRenderer.FIELD_HEIGHT + 16) / 2;

        public override void OnInitialize()
        {
            base.OnInitialize();
            gameField = new();
            Append(gameField);

            handElement = new();
            Append(handElement);

            oppHandElement = new();
            Append(oppHandElement);

            previewElement = new();
            Append(previewElement);

            actionButtons = new();
            Append(actionButtons);

            passTurnButton = new();
            Append(passTurnButton);

            cancelButton = new()
            {
                OnClickAction = SurrenderGame
            };
            Append(cancelButton);

            actionLog = new();
            Append(actionLog);

            SetRectangles(0);
        }
        public static void SetRectangle(UIElement uiElement, float left, float top, float width = 1, float height = 1)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }
        
        private void SurrenderGame()
        {
            TCGPlayer.LocalGamePlayer.Resources =
                TCGPlayer.LocalGamePlayer.Resources.UseResource(health: TCGPlayer.LocalGamePlayer.Resources.Health);
        }

        private void SetRectangles(float? lerpPoint = null)
        {
            var yOffset = MathHelper.Lerp(UI_MAX_HEIGHT, 0, lerpPoint ?? TCGPlayer.FieldTransitionPoint);
            SetRectangle(gameField, (Main.screenWidth - FieldRenderer.FIELD_WIDTH + 48) / 2, yOffset + (Main.screenHeight - FieldRenderer.FIELD_HEIGHT) / 2, 1, 1);
            SetRectangle(previewElement, gameField.Position.X - 150, gameField.Position.Y + 205, 180, 240);
            SetRectangle(handElement, Main.screenWidth / 2, yOffset + Main.screenHeight - HandElement.CARD_HEIGHT);
            SetRectangle(oppHandElement, Main.screenWidth / 2, yOffset + gameField.Position.Y);
            SetRectangle(passTurnButton, Main.screenWidth / 2 + 3 * FieldRenderer.FIELD_WIDTH / 8, yOffset + Main.screenHeight / 2);
            SetRectangle(actionLog, (Main.screenWidth + FieldRenderer.FIELD_WIDTH) / 2 + 22, yOffset + (Main.screenHeight + FieldRenderer.FIELD_HEIGHT)/2 - 104);
            SetRectangle(cancelButton, 16, Main.screenHeight - 16 - 48, 38, 48);
        }

        public override void Update(GameTime gameTime)
        {
            SetRectangles();
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Main.hoverItemName = ""; // suppress any tooltips from the main game state
            Main.HoveringOverAnNPC = false;
            base.Draw(spriteBatch);
        }
    }
}
