using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.UI.GameFieldUI;
using static TerraTCG.Common.UI.DeckbuildUI.DeckbuildCardElement;

namespace TerraTCG.Common.UI.DeckbuildUI
{
    internal class DeckbuildState : UIState
    {
        internal CancelResumeGameButton cancelButton;
        private DeckbuildCardList deckbuildCardList;
        private PlayerDeckList playerDeckList;

        const int DECKBUILD_WIDTH = 640;
        const int DECKLIST_WIDTH = 276;
        const int MIN_Y_MARGIN = 32;
        const int PADDING_Y = 16;
        public override void OnInitialize()
        {
            base.OnInitialize();
            var bgColor = new Color(73, 94, 171, 180);
            deckbuildCardList = new()
            {
                BackgroundColor = bgColor,
                PaddingLeft = 8f,
                PaddingRight = 8f,
                PaddingTop = PADDING_Y,
                PaddingBottom = PADDING_Y,
            };

            playerDeckList = new()
            {
                BackgroundColor = bgColor,
                PaddingLeft = 8f,
                PaddingRight = 8f,
                PaddingTop = PADDING_Y,
                PaddingBottom = PADDING_Y,
            };

            cancelButton = new()
            {
                OnClickAction = ()=>ModContent.GetInstance<UserInterfaces>().StopDeckbuild()
            };
            Append(cancelButton);

            Append(deckbuildCardList);

            Append(playerDeckList);

        }


        // For ease of calculating scroll, make the window height an integer number of cards
        internal static (int, int) GetWindowHeight()
        {
            var heightPerCard = (int)(CARD_HEIGHT * CARD_SCALE + CARD_MARGIN);
            var maxWindowHeight = Main.screenHeight - 2 * MIN_Y_MARGIN - 2* PADDING_Y;
            int maxRowCount = maxWindowHeight / heightPerCard;
            return (maxRowCount, maxRowCount * heightPerCard + 2 * PADDING_Y);
        }

        public override void Update(GameTime gameTime)
        {
            var windowHeight = GetWindowHeight().Item2;
            GameFieldState.SetRectangle(deckbuildCardList, 
                (Main.screenWidth - DECKBUILD_WIDTH) / 2, 
                (Main.screenHeight - windowHeight) / 2, DECKBUILD_WIDTH, windowHeight);
            GameFieldState.SetRectangle(playerDeckList,
                deckbuildCardList.Left.Pixels + deckbuildCardList.Width.Pixels + PADDING_Y,
                 (Main.screenHeight - windowHeight) / 2, DECKLIST_WIDTH, windowHeight);
            GameFieldState.SetRectangle(cancelButton, 16, Main.screenHeight - 16 - 48, 38, 48);
            base.Update(gameTime);
        }
    }
}
