using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Content.NPCs;

namespace TerraTCG.Common.UI.NPCDuelChat
{
    internal class NPCDeckSelectDialogue : UIPanel
    {
        internal NPCInfoCache NPCInfo { get; set; }
		internal int NPCID => NPCInfo.NpcId;

        internal List<NPCDuelChatButton> Buttons { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Buttons = [
                new(),
                new(),
                new(),
                new(),
            ];
            for(int i = 0; i < Buttons.Count; i++)
            {
                var localI = i;
                Buttons[i].Top.Percent = 0.85f;
                Buttons[i].OnLeftClick += (evt, elem) => SelectNPCDeck(evt, elem, localI);
                Append(Buttons[i]);
            }
        }


        private void SelectNPCDeck(UIMouseEvent evt, UIElement listeningElement, int deckIdx)
        {
            if(ModContent.GetInstance<NPCDeckMap>().NPCDecklists.TryGetValue(NPCID, out var lists))
            {
                if(deckIdx >= lists.Count)
                {
                    return;
                }

                ModContent.GetInstance<UserInterfaces>().StopNPCChat();
                if (lists[deckIdx].IsTutorial)
                {
                    ModContent.GetInstance<UserInterfaces>().StartTutorial();
                    return;
                }
                
                // Exit out of the duel dialogue if the player does not have a valid decklist
                if(!TCGPlayer.LocalPlayer.Deck.ValidateDeck())
                {
                    Main.NewText(Language.GetTextValue("Mods.TerraTCG.Cards.Common.DeckNotValid"), Color.Red);
                    ModContent.GetInstance<UserInterfaces>().StopNPCChat();
                    return;
                }

                var myPlayer = TCGPlayer.LocalPlayer;
                var opponent = new SimpleBotPlayer()
                {
                    Deck = lists[deckIdx].DeckList,
					Rewards = lists[deckIdx].Rewards,
					DeckName = lists[deckIdx].Key,
                };
                ModContent.GetInstance<GameModSystem>().StartGame(myPlayer, opponent);
				myPlayer.NPCInfo = NPCInfo;
            }
            ModContent.GetInstance<UserInterfaces>().StopNPCChat();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if(NPCID == 0 || !ModContent.GetInstance<NPCDeckMap>().NPCDecklists.TryGetValue(NPCID, out var deckLists))
            {
                return;
            }
			var unlockedLists = deckLists.Where(d => d.IsUnlocked(TCGPlayer.LocalPlayer)).ToList();
            for(int i = 0; i < unlockedLists.Count; i++)
            {
                Buttons[i].Text = unlockedLists[i].Name;
                Buttons[i].Left.Percent = i / (float)unlockedLists.Count;
				Buttons[i].Left.Pixels = 0;
            }
            for(int i = unlockedLists.Count; i < Buttons.Count;i++)
            {
                Buttons[i].Text = null;
				Buttons[i].Left.Pixels = Main.screenWidth; // Hide off of the right edge of the screen
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(NPCID == 0)
            {
                return;
            }
            base.Draw(spriteBatch);
            var innerDims = GetInnerDimensions();
            var pos = new Vector2(innerDims.X, innerDims.Y);
            var font = FontAssets.MouseText.Value;
            var color = Main.MouseTextColorReal; ;
            var dialogueText = Language.GetTextValue("Mods.TerraTCG.Cards.Common.AskDeck");
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, dialogueText, pos, color: color, font: font);
        }
    }
}
