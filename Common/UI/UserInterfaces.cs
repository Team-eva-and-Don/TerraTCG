using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TerraTCG.Common.UI.DeckbuildUI;
using TerraTCG.Common.UI.GameFieldUI;
using TerraTCG.Common.UI.NPCDuelChat;

namespace TerraTCG.Common.UI
{
    [Autoload(true, Side = ModSide.Client)]
    internal class UserInterfaces : ModSystem
    {
        private UserInterface _userInterface;

        private GameFieldState GameField { get; set; }

        private NPCDuelChatState DuelChat { get; set; }

        private DeckbuildState DeckbuildState { get; set; }

        public override void Load()
        {
            GameField = new();
            GameField.Activate();

            DuelChat = new();
            DuelChat.Activate();

            DeckbuildState = new();
            DeckbuildState.Activate();

            _userInterface = new();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
            _userInterface.Update(gameTime);
        }

        public void StartGame()
        {
            IngameFancyUI.OpenUIState(GameField);
        }

        public void EndGame()
        {
            IngameFancyUI.Close();
            Main.playerInventory = false;
        }
        
        public void StartNPCChat()
        {
            _userInterface.SetState(DuelChat);
        }

        public void StopNPCChat()
        {
            _userInterface.SetState(null);
        }

        public void StartDeckbuild()
        {
            _userInterface.SetState(DeckbuildState);
            // IngameFancyUI.OpenUIState(DeckbuildState);
        }
        public void StopDeckbuild()
        {
            _userInterface.SetState(null);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIdx = layers.FindIndex(layer => layer.Name.Equals("Vanilla: NPC / Sign Dialog"));
            if(mouseTextIdx != -1 && _userInterface?.CurrentState == DuelChat)
            {
                layers.Insert(mouseTextIdx+1, new LegacyGameInterfaceLayer("TerraTCG: Duel Dialog", delegate
                {
                    if(_userInterface?.CurrentState != null)
                    {
                        _userInterface.Draw(Main.spriteBatch, new GameTime());
                    }
                    return true;
                }, InterfaceScaleType.UI));
            } else if (_userInterface?.CurrentState == DeckbuildState)
            {
                layers.Insert(mouseTextIdx+1, new LegacyGameInterfaceLayer("TerraTCG: Deckbuilder", delegate
                {
                    if(_userInterface?.CurrentState != null)
                    {
                        _userInterface.Draw(Main.spriteBatch, new GameTime());
                    }
                    return true;
                }, InterfaceScaleType.UI));
            }
        }
    }
}
