using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.UI.GameFieldUI;
using TerraTCG.Common.UI.NPCDuelChat;

namespace TerraTCG.Common.UI.TutorialUI
{
    internal struct TutorialSlide(int imageIdx, int textIdx)
    {
        public readonly LocalizedText Text => Language.GetText($"Mods.TerraTCG.Cards.Tutorial.Slide{textIdx}");
        public readonly Asset<Texture2D> Texture => TextureCache.Instance.TutorialSlides[imageIdx];

    }
    internal class TutorialUIState : UIState
    {
        public const float TUTORIAL_IMG_SCALE = 0.75f;
        const int TUTORIAL_IMG_WIDTH = 817;
        const int TUTORIAL_IMG_HEIGHT = 736;
        const int PADDING = 16;

        private TutorialUIPanel tutorialPanel;
        private NPCDuelChatButton prevButton;
        private NPCDuelChatButton nextButton;

        private TimeSpan clickDebounceTime;

        static int WindowWidth => (int)(TUTORIAL_IMG_SCALE * TUTORIAL_IMG_WIDTH) + 2 * PADDING;
        static int WindowHeight => (int)(TUTORIAL_IMG_SCALE * TUTORIAL_IMG_HEIGHT) + 2 * PADDING + 148;


        public List<TutorialSlide> Slides = [
            new(0, 0),
            new(1, 1),
            new(1, 2),
            new(14, 3),
            new(1, 4),
            new(2, 5),
            new(3, 6),
            new(4, 7),
            new(6, 8),
            new(8, 9),
            new(9, 10),
            new(11, 11),
            new(12, 12),
            new(13, 13),
            new(14, 14),
            new(15, 15),
            new(16, 16),
            new(17, 17),
            new(0, 18),
        ];
        public int SlideIdx { get; set; }

        public TutorialSlide CurrentSlide => Slides[SlideIdx];

        public override void OnInitialize()
        {
            tutorialPanel = new TutorialUIPanel()
            {
                PaddingTop = PADDING,
                PaddingBottom = 2 * PADDING,
                PaddingLeft = PADDING,
                PaddingRight = PADDING,
            };

            Append(tutorialPanel);

            prevButton = new NPCDuelChatButton();
            nextButton = new NPCDuelChatButton();

            prevButton.Top.Percent = 1;
            prevButton.Left.Percent = 0;
            nextButton.Top.Percent = 1;
            nextButton.Left.Percent = 0.95f;

            tutorialPanel.Append(prevButton);
            tutorialPanel.Append(nextButton);

            prevButton.OnLeftClick += PrevButton_OnLeftClick;
            nextButton.OnLeftClick += NextButton_OnLeftClick;
        }

        private void NextButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if(TCGPlayer.TotalGameTime - clickDebounceTime < TimeSpan.FromSeconds(0.25f))
            {
                return;
            }
            clickDebounceTime = TCGPlayer.TotalGameTime;
            if(SlideIdx == Slides.Count - 1)
            {
                ModContent.GetInstance<UserInterfaces>().StopTutorial();
            } else
            {
                tutorialPanel.PrevTexture = CurrentSlide.Texture;
                SlideIdx += 1;
            }
        }

        private void PrevButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if(TCGPlayer.TotalGameTime - clickDebounceTime < TimeSpan.FromSeconds(0.25f))
            {
                return;
            }
            clickDebounceTime = TCGPlayer.TotalGameTime;
            if(SlideIdx == 0)
            {
                ModContent.GetInstance<UserInterfaces>().StopTutorial();
            } else
            {
                tutorialPanel.PrevTexture = CurrentSlide.Texture;
                SlideIdx -= 1;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            tutorialPanel.Texture = CurrentSlide.Texture;
            tutorialPanel.Text = CurrentSlide.Text.Value;

            prevButton.Text = Language.GetText($"Mods.TerraTCG.Cards.Common.{(SlideIdx == 0 ? "Close": "Prev")}");
            nextButton.Text = Language.GetText($"Mods.TerraTCG.Cards.Common.{(SlideIdx == Slides.Count -1 ? "Close": "Next")}");

            GameFieldState.SetRectangle(tutorialPanel,
                (Main.screenWidth  - WindowWidth) / 2,
                (Main.screenHeight - WindowHeight) / 2,
                WindowWidth,
                WindowHeight);
        }

        internal void ResetState()
        {
            SlideIdx = 0;
        }
    }
}
