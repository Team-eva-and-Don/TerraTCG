using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.UI.DeckbuildUI;
using TerraTCG.Common.UI.GameFieldUI;
using TerraTCG.Common.UI.NPCDuelChat;
using TerraTCG.Common.UI.PackOpeningUI;

namespace TerraTCG.Common.UI
{
    [Autoload(true, Side = ModSide.Client)]
    internal class UserInterfaces : ModSystem
    {
        private UserInterface _userInterface;

        private GameFieldState GameField { get; set; }

        private NPCDuelChatState DuelChat { get; set; }

        private DeckbuildState DeckbuildState { get; set; }

        private PackOpeningState PackState { get; set; }

        public override void Load()
        {
            GameField = new();
            GameField.Activate();

            DuelChat = new();
            DuelChat.Activate();

            DeckbuildState = new();
            DeckbuildState.Activate();

            PackState = new();
            PackState.Activate();

            _userInterface = new();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
            _userInterface.Update(gameTime);

            // TODO is this the correct place to put this?
            // Close out all interfaces if ESC is pressed
            if(Main.LocalPlayer.controlInv)
            {
                TCGPlayer.LocalGamePlayer?.Surrender();
                if(_userInterface.CurrentState == DuelChat)
                {
                    StopNPCChat();
                }
                if(_userInterface.CurrentState == DeckbuildState)
                {
                    StopDeckbuild();
                }
            }
        }

        public void StartGame()
        {
            IngameFancyUI.OpenUIState(GameField);
        }

        public void EndGame()
        {
            IngameFancyUI.Close();
        }
        
        public void StartPackOpening()
        {
            PackState.StartTime = TCGPlayer.TotalGameTime;
            _userInterface.SetState(PackState);
        }

        public bool IsPackOpening() => _userInterface.CurrentState == PackState;

        public void StopPackOpening()
        {
            _userInterface.SetState(null);
        }

        public void StartNPCChat()
        {
            _userInterface.SetState(DuelChat);
        }

        public void StopNPCChat()
        {
            DuelChat.ResetState();
            _userInterface.SetState(null);
        }

        public void StartDeckbuild()
        {
            if(_userInterface.CurrentState != DeckbuildState)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                _userInterface.SetState(DeckbuildState);
            }
        }
        public void StopDeckbuild()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            _userInterface.SetState(null);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIdx = layers.FindIndex(layer => layer.Name.Equals("Vanilla: NPC / Sign Dialog"));
            if(mouseTextIdx != -1 && _userInterface?.CurrentState != null)
            {
                var layerName = $"TerraTCG: {_userInterface.CurrentState}";
                layers.Insert(mouseTextIdx+1, new LegacyGameInterfaceLayer(layerName, delegate
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
