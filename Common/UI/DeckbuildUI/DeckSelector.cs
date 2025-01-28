using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;

namespace TerraTCG.Common.UI.DeckbuildUI
{
    internal class DeckSelector : UIPanel
    {
        private List<DeckSelectorButton> buttonList;
        private const int DECK_COUNT = 8;
        public override void OnInitialize()
        {
            buttonList = [];

            for(int i = 0; i < DECK_COUNT; i++)
            {
                var localI = i;
                var btn = new DeckSelectorButton()
                {
                    DeckIdx = localI,
                };
                btn.Top.Percent = 0.5f;
                btn.Left.Percent = i / (float)DECK_COUNT;
                btn.OnLeftClick += (evt, elem) => OnClickButton(localI);

                Append(btn);
                buttonList.Add(btn);
            }
        }

        private void OnClickButton(int deckIdx)
        {
            // TODO referencing sibling elements like this is not the preferred approach
            ((DeckbuildState)Parent).playerDeckList.ResetScroll();
            TCGPlayer.LocalPlayer.ActiveDeck = deckIdx;
        }

        public override void Update(GameTime gameTime)
        {
            Main.LocalPlayer.mouseInterface = true;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var textPos = new Vector2(GetInnerDimensions().X, GetInnerDimensions().Y);
            var font = FontAssets.MouseText.Value;
            var text = Language.GetText("Mods.TerraTCG.Cards.Common.DeckEdit").Format($"{TCGPlayer.LocalPlayer.ActiveDeck + 1}");
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, text, textPos, font: font);

            var deckCountLabel = Language.GetTextValue("Mods.TerraTCG.Cards.Common.DeckCount");

            var deckCountText = $"{deckCountLabel}: {TCGPlayer.LocalPlayer.Deck.Cards.Count}/20";
            var deckCountColor = TCGPlayer.LocalPlayer.Deck.Cards.Count == 20 ? Color.White : Color.LightCoral;

            var deckCountWidth = font.MeasureString(deckCountText).X;
            var deckCountPos = textPos + new Vector2(GetInnerDimensions().Width - deckCountWidth, 0);
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, deckCountText, deckCountPos, color: deckCountColor, font: font);

        }
    }
}
