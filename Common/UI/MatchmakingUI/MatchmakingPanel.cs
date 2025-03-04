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
using TerraTCG.Common.Netcode;
using TerraTCG.Common.UI.NPCDuelChat;

namespace TerraTCG.Common.UI.MatchmakingUI
{
    internal class MatchmakingPanel : UIPanel
    {
		private NPCDuelChatButton hostButton;

		private Vector2 mouseOffsetVector;

		private bool isDragging = false;

        public override void OnInitialize()
        {
            hostButton = new NPCDuelChatButton()
			{
				Text = Language.GetText("Mods.TerraTCG.Cards.Common.Host"),
				Top = {Pixels = 0 },
				Left = { Percent = 0.92f },
			};
			hostButton.OnLeftClick += HostButton_OnLeftClick;

			OnLeftMouseDown += MatchmakingPanel_OnLeftMouseDown;
			Append(hostButton);
        }

		// Draggable UI Panel
		private void MatchmakingPanel_OnLeftMouseDown(UIMouseEvent evt, UIElement listeningElement)
		{
			isDragging = true;
			mouseOffsetVector = new(Left.Pixels - Main.MouseScreen.X, Top.Pixels - Main.MouseScreen.Y); 
		}

		private void HostButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			var syncPlayer = Main.LocalPlayer.GetModPlayer<GameStateSyncPlayer>();
			syncPlayer.LookingForGame = !syncPlayer.LookingForGame;
		}

		public override void Update(GameTime gameTime)
        {
            Main.LocalPlayer.mouseInterface = true;
			var syncPlayer = Main.LocalPlayer.GetModPlayer<GameStateSyncPlayer>();
			if(syncPlayer.LookingForGame)
			{
				hostButton.Text = Language.GetText("Mods.TerraTCG.Cards.Common.Cancel");
				hostButton.Left.Percent = 0.89f;
			} else
			{
				hostButton.Text = Language.GetText("Mods.TerraTCG.Cards.Common.Host");
				hostButton.Left.Percent = 0.92f;
			}

			// TODO do we want to make a DraggablePanel base class?
			if(Main.mouseLeft && isDragging)
			{
				Left.Pixels = Main.MouseScreen.X + mouseOffsetVector.X;
				Top.Pixels = Main.MouseScreen.Y + mouseOffsetVector.Y;
			} else
			{
				isDragging = false;
			}
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var textPos = new Vector2(GetInnerDimensions().X, GetInnerDimensions().Y);
            var font = FontAssets.MouseText.Value;
			var text = Language.GetTextValue("Mods.TerraTCG.Cards.Common.Matchmaking");
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, text, textPos, font: font);
        }
    }
}
