using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI.GameFieldUI;

namespace TerraTCG.Common.UI.NPCDuelChat
{
    internal class NPCDuelChatState : UIState
    {
        private NPCDuelChatButton chatButton;

        public override void OnInitialize()
        {
            base.OnInitialize();
            chatButton = new NPCDuelChatButton();
            chatButton.OnLeftClick += OnClickChatButton;
            Append(chatButton);
        }

        private void OnClickChatButton(UIMouseEvent evt, UIElement listeningElement)
        {
            var myPlayer = TCGPlayer.LocalPlayer;
            var opponent = new SimpleBotPlayer();

            myPlayer.Deck = BotDecks.GetDeck();
            opponent.Deck = BotDecks.GetDeck();
            Main.CloseNPCChatOrSign();
            ModContent.GetInstance<UserInterfaces>().StopNPCChat();
            ModContent.GetInstance<GameModSystem>().StartGame(myPlayer, opponent);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var font = FontAssets.MouseText.Value;
            var lines = Utils.WordwrapString(Main.npcChatText, font, 460, 10, out var lineCount);
            var yOffset = lines.Where(l=>l != null).Select(l => font.MeasureString(l).Y).Sum();
            var buttonText = Language.GetTextValue("Mods.TerraTCG.Common.DuelChat");
            var buttonSize = font.MeasureString(buttonText);
            var xOffset = buttonSize.X;

            // TODO there are probably a number of factors that prevent this
            // from aligning the text correctly in all scenarios.
            GameFieldState.SetRectangle(chatButton, Main.screenWidth / 2 + 184 - xOffset, 134 + yOffset, buttonSize.X, buttonSize.Y);
        }
    }
}
