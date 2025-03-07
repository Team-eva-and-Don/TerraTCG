﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI.GameFieldUI;
using static System.Net.Mime.MediaTypeNames;
using static TerraTCG.Common.UI.DeckbuildUI.DeckbuildCardElement;

namespace TerraTCG.Common.UI.DeckbuildUI
{

    internal class DeckbuildState : UIState
    {
        internal CancelResumeGameButton cancelButton;
        private DeckbuildCardList deckbuildCardList;
        internal PlayerDeckList playerDeckList;
        private DeckSelector deckSelector;
        private CardListFilter cardListFilter;
        private CardPreviewElement cardPreview;

        const int DECKBUILD_WIDTH = 616;
        const int DECKLIST_WIDTH = 304;
        const int MIN_TOP_MARGIN = 140;
        const int MIN_BOTTOM_MARGIN = 16;
        const int PADDING_Y = 16;
        public override void OnInitialize()
        {
            base.OnInitialize();
            var bgColor = new Color(54, 53, 131, 210);
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

            deckSelector = new()
            {
                BackgroundColor = bgColor,
                PaddingLeft = 8f,
                PaddingRight = 8f,
                PaddingTop = 8f,
                PaddingBottom = 8f,
            };

            cardListFilter = new()
            {
                BackgroundColor = bgColor,
                PaddingLeft = 8f,
                PaddingRight = 8f,
                PaddingTop = 8f,
                PaddingBottom = 8f,
            };

            cancelButton = new()
            {
                OnClickAction = ()=>ModContent.GetInstance<UserInterfaces>().StopDeckbuild()
            };


            cardPreview = new(); 

            Append(cancelButton);

            Append(deckbuildCardList);

            Append(playerDeckList);

            Append(deckSelector);

            Append(cardListFilter);

            Append(cardPreview);

        }

        internal IEnumerable<T> FilterCards<T>(IEnumerable<T> cardList) where T : IHasCard => cardListFilter.FilterCardContainer(cardList);

        // Flag to prevent Draw() from running before element dimensions are calculated in Update()
        public bool IsOpen { get; internal set; }


        // For ease of calculating scroll, make the window height an integer number of cards
        internal static (int, int) GetWindowHeight()
        {
            var heightPerCard = (int)(CARD_HEIGHT * CARD_SCALE + CARD_MARGIN);
            var maxWindowHeight = Main.screenHeight - MIN_TOP_MARGIN - MIN_BOTTOM_MARGIN - 2* PADDING_Y;
            int maxRowCount = maxWindowHeight / heightPerCard;
            return (maxRowCount, maxRowCount * heightPerCard + 2 * PADDING_Y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(IsOpen)
            {
                base.Draw(spriteBatch);
            }
        }
        public override void Update(GameTime gameTime)
        {
            var windowHeight = GetWindowHeight().Item2;
            GameFieldState.SetRectangle(deckbuildCardList, 
                (Main.screenWidth - DECKBUILD_WIDTH - DECKLIST_WIDTH) / 2, 
                MIN_TOP_MARGIN, DECKBUILD_WIDTH, windowHeight);

            GameFieldState.SetRectangle(playerDeckList,
                deckbuildCardList.Left.Pixels + deckbuildCardList.Width.Pixels + PADDING_Y,
                MIN_TOP_MARGIN, DECKLIST_WIDTH, windowHeight);
            
            GameFieldState.SetRectangle(deckSelector,
                playerDeckList.Left.Pixels, playerDeckList.Top.Pixels - 68, DECKLIST_WIDTH, 64);

            GameFieldState.SetRectangle(cardListFilter,
                deckbuildCardList.Left.Pixels, deckbuildCardList.Top.Pixels - 68, DECKBUILD_WIDTH, 64);

            GameFieldState.SetRectangle(cardPreview,
                deckbuildCardList.Left.Pixels - 186, deckbuildCardList.Top.Pixels + windowHeight/2 - 120, 180, 240);

            GameFieldState.SetRectangle(cancelButton, 16, Main.screenHeight - 16 - 48, 38, 48);

            IsOpen = true;
            base.Update(gameTime);
        }

        public static void SetTooltip(string tooltip, int rare = 0)
        {
            Main.LocalPlayer.cursorItemIconEnabled = false;
            Main.ItemIconCacheUpdate(0);
            // via tModloader, need to set Rare as well
            if (Main.SettingsEnabled_OpaqueBoxBehindTooltips)
            {
                Item fakeItem = new ();
                fakeItem.SetDefaults(0, noMatCheck: true);
                fakeItem.SetNameOverride(tooltip);
                fakeItem.type = 1;
                fakeItem.scale = 0f;
                fakeItem.rare = rare;
                fakeItem.value = -1;
                Main.HoverItem = fakeItem;
                Main.instance.MouseText("");
                Main.mouseText = true;
            }
            else
            {
                Main.instance.MouseText(tooltip, rare);
            }
        }

        public void ResetState()
        {
            deckbuildCardList.ResetScroll();
            playerDeckList.ResetScroll();
        }
    }
}
