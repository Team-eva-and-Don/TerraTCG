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
using TerraTCG.Content.NPCs;

namespace TerraTCG.Common.UI.NPCDuelChat
{
    internal class NPCDuelChatState : UIState
    {
        // Button that attaches itself to the "real" NPC chat that allows starting a game
        private NPCDuelChatButton chatButton;

        // Menu replicating NPC dialog that appears after the NPC is done talking
        private NPCDeckSelectDialogue dialog;

        private int talkNPCIdx = -1;

        public override void OnInitialize()
        {
            base.OnInitialize();
            chatButton = new NPCDuelChatButton()
            {
                Text = Language.GetText("Mods.TerraTCG.Cards.Common.DuelChat")
            };
            chatButton.OnLeftClick += OnClickChatButton;

            var bgColor = new Color(54, 53, 131, 210);

            var borderColor = new Color(8, 8, 31, 210);
            dialog = new NPCDeckSelectDialogue()
            {
                BackgroundColor = bgColor,
                BorderColor = borderColor,
            };
            dialog.SetPadding(16);

            Append(dialog);
            Append(chatButton);
        }

        private void OnClickChatButton(UIMouseEvent evt, UIElement listeningElement)
        {
            AdvanceToDeckSelectDialogue();
        }

        public void AdvanceToDeckSelectDialogue()
        {
            dialog.NPCInfo = new(Main.LocalPlayer.TalkNPC);
            talkNPCIdx = Main.LocalPlayer.talkNPC;
            Main.CloseNPCChatOrSign();
        }

        private void UpdateDuelStartButtonPosition()
        {
            if(dialog.NPCID != 0 || !ModContent.GetInstance<UserInterfaces>().VanillaDialogueLayerActive)
            {
                GameFieldState.SetRectangle(chatButton, Main.screenWidth + 20, 0);
                return;
            }
            // Compute the height of the NPC's dialogue
            var font = FontAssets.MouseText.Value;
            var lines = Utils.WordwrapString(Main.npcChatText, font, 460, 10, out var lineCount);
            var yOffset = 30 * lines.Where(l => l != null).Count();

            var height = chatButton.Height.Pixels;
            var width = chatButton.Width.Pixels;

            // TODO there are probably a number of factors that prevent this
            // from aligning the text correctly in all scenarios.
            GameFieldState.SetRectangle(chatButton, Main.screenWidth / 2 + 184 - width, 130 + yOffset, width, height);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if((Main.npcChatText?.Length ?? 0) > 0)
            {
                dialog.NPCInfo = default; // Reset duel start progress if we talk to another NPC
				talkNPCIdx = -1;
            } else if ((Main.npcChatText?.Length ?? 0) == 0 && dialog.NPCID == 0)
            {
                // Chat closed before launching the duel dialog, exit out of this UI state
                ModContent.GetInstance<UserInterfaces>().StopNPCChat();
                return;
            }

            if(talkNPCIdx != -1)
            {
                var localPlayer = Main.LocalPlayer;
                if (Main.npc[talkNPCIdx] is NPC npc && npc.active)
                {
                    var interactRect = Utils.CenteredRectangle(localPlayer.Center, new Vector2(Player.tileRangeX, Player.tileRangeY) * 32);
                    // Player moved away from NPC, exit out of this UI state
                    // TODO the logic for canceling this UI is getting a bit convoluted
                    if(!interactRect.Intersects(npc.Hitbox))
                    {
                        ModContent.GetInstance<UserInterfaces>().StopNPCChat();
                        return;
                    }
                }
                Main.LocalPlayer.SetTalkNPC(talkNPCIdx);
            }

            if(dialog.NPCID == 0)
            {
                GameFieldState.SetRectangle(dialog, 2 * Main.screenWidth, 120, 460, 120);
            } else
            {
                GameFieldState.SetRectangle(dialog, (Main.screenWidth - 500)/2 , 100, 500, 120);
            }
            UpdateDuelStartButtonPosition();
        }

        internal bool InDeckSelect => dialog.NPCID > 0;
        internal void ResetState()
        {
            // This can sometimes lead to a state where an NPC is stuck talking to
            // a player after all dialogues are closed, attempt to clear that
            if(talkNPCIdx != -1 && (Main.npcChatText?.Length ?? 0) == 0)
            {
                Main.LocalPlayer.SetTalkNPC(-1);
            }
            talkNPCIdx = -1;
			dialog.NPCInfo = default;
        }
    }
}
