using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations;
using TerraTCG.Common.Netcode;
using TerraTCG.Common.Netcode.Packets;
using TerraTCG.Common.UI.DeckbuildUI;
using TerraTCG.Common.UI.GameFieldUI;
using TerraTCG.Common.UI.MatchmakingUI;
using TerraTCG.Common.UI.NPCDuelChat;
using TerraTCG.Common.UI.PackOpeningUI;
using TerraTCG.Common.UI.TutorialUI;

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

        private TutorialUIState TutorialState { get; set; }

        private MatchmakingUIState MatchmakingState { get; set; }

        private bool? CachedAutoPause;

        // For DialogueTweaks support, this can be checked by UIState code
        // to see whether vanilla UI is currently being suppressed
        public bool VanillaDialogueLayerActive { get; private set; }

		public bool MatchmakingLayerActive => _userInterface.CurrentState == MatchmakingState;

        public override void OnModLoad()
        {
            GameField = new();
            GameField.Activate();

            DuelChat = new();
            DuelChat.Activate();

            DeckbuildState = new();
            DeckbuildState.Activate();

            PackState = new();
            PackState.Activate();

            TutorialState = new();
            TutorialState.Activate();

			MatchmakingState = new();
			MatchmakingState.Activate();

            _userInterface = new();
            On_Player.OpenInventory += On_Player_OpenInventory;

			On_IngameFancyUI.Close += On_IngameFancyUI_Close;

			On_Main.GUIBarsDraw += On_Main_GUIBarsDraw;

			On_Main.DrawMouseOver += On_Main_DrawMouseOver;
        }

		private void On_Main_DrawMouseOver(On_Main.orig_DrawMouseOver orig, Main self)
		{
			// Hack to display (Playing TerraTCG) when mousing over an in-game player
			var mouseOverInGamePlayer = Main.player
				// Vanilla conditions for displaying a tooltip over a player
				.Where(p => p.active && p.whoAmI != Main.myPlayer && !p.dead && !p.ShouldNotDraw && p.stealth < 0.5f)
				.Where(p =>
				{
					var hb = p.Hitbox;
					hb.Inflate(10, 10);
					return hb.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y);
				})
				.Where(p => p.GetModPlayer<GameStateSyncPlayer>().InGame)
				.FirstOrDefault();
			var realName = mouseOverInGamePlayer?.name;
			if(mouseOverInGamePlayer != null)
			{
				mouseOverInGamePlayer.name = $"{realName} ({Language.GetTextValue("Mods.TerraTCG.Cards.Common.PlayingTerraTCG")})";
			}
			
			orig.Invoke(self);

			if(mouseOverInGamePlayer != null)
			{
				mouseOverInGamePlayer.name = realName;
			}
		}

		private void On_Main_GUIBarsDraw(On_Main.orig_GUIBarsDraw orig, Main self)
		{
			// Don't draw the player health/breath bards while in-game
			if(TCGPlayer.LocalGamePlayer == null)
			{
				orig.Invoke(self);
				return;
			}
		}

		private void On_IngameFancyUI_Close(On_IngameFancyUI.orig_Close orig)
		{
			// Make the game go through the concede animation rather than suddenly
			// disappearing when the player presses esc
			if(TCGPlayer.LocalGamePlayer == null)
			{
				orig();
				return;
			}
			if(TCGPlayer.LocalGamePlayer.Game.FieldAnimation is QuitNotificationAnimation)
			{
				if(TCGPlayer.LocalGamePlayer?.Game.IsMultiplayer ?? false)
				{
					GameActionPacketQueue.Instance.QueueOutgoingMessage(new SurrenderPacket(Main.LocalPlayer));
				}
				TCGPlayer.LocalGamePlayer.Surrender();
			} else
			{
				TCGPlayer.LocalGamePlayer.Game.FieldAnimation =
					new QuitNotificationAnimation(TCGPlayer.TotalGameTime);
			}
		}

		private void On_Player_OpenInventory(On_Player.orig_OpenInventory orig)
        {
            // Stop the player from opening the inventory while 
            // the deckbuilder is open (or in the process of clothing)
            // TODO it feels a bit high-risk to conditionally turn off vanilla
            // inventory opening
            if(_userInterface.CurrentState != DeckbuildState)
            {
                orig.Invoke();
            }
        }

        public override void PreUpdatePlayers()
        {
            base.PreUpdatePlayers();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
            _userInterface.Update(gameTime);

            // TODO is this the correct place to put this?
            // Close out all interfaces if ESC is pressed
            if(Main.LocalPlayer.controlInv)
            {
                // TCGPlayer.LocalGamePlayer?.Surrender();
                //if(CachedAutoPause is bool cachedAutoPause)
                //{
                //    Main.autoPause = cachedAutoPause;
                //}

                if(_userInterface.CurrentState == DuelChat && DuelChat.InDeckSelect)
                {
                    StopNPCChat();
                }
                if(_userInterface.CurrentState == DeckbuildState)
                {
                    StopDeckbuild();
                }
                if(_userInterface.CurrentState == TutorialState)
                {
                    StopTutorial();
                }
				if(_userInterface.CurrentState == MatchmakingState)
				{
					StopMatchmaking();
				}
            }
        }

        public void StartGame()
        {
            CachedAutoPause = Main.autoPause;
            Main.autoPause = true;
            IngameFancyUI.OpenUIState(GameField);
        }

        public void EndGame()
        {
            IngameFancyUI.Close();
            if(CachedAutoPause is bool cachedAutoPause)
            {
                Main.autoPause = cachedAutoPause;
            }
            Main.playerInventory = false;
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

        public void StartTutorial()
        {
            TextureCache.Instance.LoadTutorial();
            _userInterface.SetState(TutorialState);
        }

        public void StopTutorial()
        {
            TutorialState.ResetState();
            _userInterface.SetState(null);
        }

        public void StartDeckbuild()
        {
            if(_userInterface.CurrentState != DeckbuildState)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                _userInterface.SetState(DeckbuildState);
                Main.playerInventory = false;
            }
        }
        public void StopDeckbuild()
        {
            DeckbuildState.IsOpen = false;
            DeckbuildState.ResetState();
            SoundEngine.PlaySound(SoundID.MenuClose);
            _userInterface.SetState(null);

            Main.playerInventory = false;
        }

		internal void StartMatchmaking()
		{
			if(_userInterface.CurrentState != MatchmakingState)
			{
				SoundEngine.PlaySound(SoundID.MenuOpen);
				_userInterface.SetState(MatchmakingState);
				var syncPlayer = Main.LocalPlayer.GetModPlayer<GameStateSyncPlayer>();
				syncPlayer.LookingForGame = true;
				syncPlayer.BroadcastSyncState();
			}
		}

		internal void StopMatchmaking()
		{
            DeckbuildState.IsOpen = false;
            DeckbuildState.ResetState();
            SoundEngine.PlaySound(SoundID.MenuClose);
            _userInterface.SetState(null);

			var syncPlayer = Main.LocalPlayer.GetModPlayer<GameStateSyncPlayer>();
			syncPlayer.LookingForGame = false;
			syncPlayer.BroadcastSyncState();
		}

        internal void AdvanceChat()
        {
            DuelChat.AdvanceToDeckSelectDialogue();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // Hide the mouse text layer while the game UI is active, otherwise eg.
            // NPC names will show up above the dialogue
            if(_userInterface?.CurrentState == TutorialState)
            {
                var mouseoverIdx = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Over"));
                if(mouseoverIdx != -1)
                {
                    layers.RemoveAt(mouseoverIdx);
                }
            }
            int dialogueIdx = layers.FindIndex(layer => layer.Name.Equals("Vanilla: NPC / Sign Dialog"));
            VanillaDialogueLayerActive = dialogueIdx > -1 && layers[dialogueIdx].Active;

            if(dialogueIdx != -1 && _userInterface?.CurrentState != null)
            {
                var layerName = $"TerraTCG: {_userInterface.CurrentState}";
                layers.Insert(dialogueIdx+1, new LegacyGameInterfaceLayer(layerName, delegate
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
