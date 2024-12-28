using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
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

        public override void Load()
        {
            GameField = new();
            GameField.Activate();

            DuelChat = new();
            DuelChat.Activate();

            _userInterface = new();
            _userInterface.SetState(DuelChat);
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

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIdx = layers.FindIndex(layer => layer.Name.Equals("Vanilla: NPC / Sign Dialog"));
            if(mouseTextIdx != -1 && Main.npcChatText != "")
            {
                layers.Insert(mouseTextIdx+1, new LegacyGameInterfaceLayer("TerraTCG: Duel Dialog", delegate
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
