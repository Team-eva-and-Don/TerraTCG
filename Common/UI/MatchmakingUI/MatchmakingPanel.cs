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
using Terraria.ModLoader;
using Terraria.UI;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.Netcode;
using TerraTCG.Common.Netcode.Packets;
using TerraTCG.Common.UI.NPCDuelChat;

namespace TerraTCG.Common.UI.MatchmakingUI
{
    internal class MatchmakingPanel : UIPanel
    {
		public const int MAX_OPPONENTS = 5;

		private NPCDuelChatButton cancelButton;

		private readonly NPCDuelChatButton[] joinButtons = new NPCDuelChatButton[MAX_OPPONENTS];

		private Vector2 mouseOffsetVector;

		private bool isDragging = false;


		public static List<GameStateSyncPlayer> LookingForGamePlayers =>
			Main.player.Where(p => p.active && p.whoAmI != Main.myPlayer)
				.OrderBy(p => Vector2.DistanceSquared(p.Center, Main.LocalPlayer.Center))
				.Select(p => p.GetModPlayer<GameStateSyncPlayer>())
				.Where(p => p.LookingForGame)
				.Take(MAX_OPPONENTS)
				.ToList();

		private List<GameStateSyncPlayer> lookingForGamePlayers; 

		private float GetPlayerRowYOffset(int row)
		{
            var font = FontAssets.MouseText.Value;
			var text = Language.GetTextValue("Mods.TerraTCG.Cards.Common.Matchmaking");
			var textHeight = font.MeasureString(text).Y;
			return textHeight + 16 + 48 * row - textHeight / 4; 
		}

        public override void OnInitialize()
        {
            cancelButton = new NPCDuelChatButton()
			{
				Text = Language.GetText("Mods.TerraTCG.Cards.Common.Cancel"),
				Top = {Pixels = 0 },
				Left = { Percent = 0.89f },
			};

			for(int i = 0; i < MAX_OPPONENTS; i++)
			{
				var localI = i;
				joinButtons[i] = new NPCDuelChatButton()
				{
					Text = Language.GetText("Mods.TerraTCG.Cards.Common.Join"),
					Top = {Pixels = GetPlayerRowYOffset(i) },
					Left = { Percent = 0.92f },
				};
				joinButtons[i].OnLeftClick += (evt, el) => AcceptGame(localI);
				Append(joinButtons[i]);
			}
			cancelButton.OnLeftClick += CancelButton_OnLeftClick;

			OnLeftMouseDown += MatchmakingPanel_OnLeftMouseDown;
			Append(cancelButton);
        }

		// Draggable UI Panel
		private void MatchmakingPanel_OnLeftMouseDown(UIMouseEvent evt, UIElement listeningElement)
		{
			isDragging = true;
			mouseOffsetVector = new(Left.Pixels - Main.MouseScreen.X, Top.Pixels - Main.MouseScreen.Y); 
		}

		public void AcceptGame(int playerIdx)
		{
			isDragging = false;
			ModContent.GetInstance<UserInterfaces>().StopMatchmaking();

			var opponentId = LookingForGamePlayers[playerIdx].Player.whoAmI;
			var myPlayer = TCGPlayer.LocalPlayer;
			var opponentController = new NoOpNetGamePlayerController(); // Replaced with real opponent during deck sync
			// TODO allowing the client to decide who goes first is not great
			var goingFirst = Main.rand.Next() % 2;


			ModContent.GetInstance<GameModSystem>().StartGame(myPlayer, opponentController, goingFirst);
			// Send out a net packet to trigger a game with another player
			var handAndDeck = new CardCollection()
			{
				Cards = [.. myPlayer.GamePlayer.Deck.Cards, .. myPlayer.GamePlayer.Hand.Cards]
			};
			GameActionPacketQueue.Instance.QueueOutgoingMessage(
				new DecklistPacket(myPlayer.Player, opponentId, goingFirst, handAndDeck));
		}

		private void CancelButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			ModContent.GetInstance<UserInterfaces>().StopMatchmaking();
			isDragging = false;
		}

		public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
			// Auto-close if we start a game
			if(TCGPlayer.LocalGamePlayer != null)
			{
				ModContent.GetInstance<UserInterfaces>().StopMatchmaking();
				return;
			}

            Main.LocalPlayer.mouseInterface = true;

			// TODO do we want to make a DraggablePanel base class?
			if(Main.mouseLeft && isDragging)
			{
				Left.Pixels = Main.MouseScreen.X + mouseOffsetVector.X;
				Top.Pixels = Main.MouseScreen.Y + mouseOffsetVector.Y;
			} else
			{
				isDragging = false;
			}

			lookingForGamePlayers = LookingForGamePlayers;

			for(int i = 0; i < MAX_OPPONENTS; i++)
			{
				if(i >= lookingForGamePlayers.Count || lookingForGamePlayers[i].Player.whoAmI == Main.myPlayer)
				{
					joinButtons[i].Text = null;
					joinButtons[i].Left.Percent = 1000;
				} else
				{
					joinButtons[i].Text = Language.GetText("Mods.TerraTCG.Cards.Common.Join");
					joinButtons[i].Left.Percent = 0.92f;
				}
			}
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var textPos = new Vector2(GetInnerDimensions().X, GetInnerDimensions().Y);
            var font = FontAssets.MouseText.Value;
			var text = Language.GetTextValue("Mods.TerraTCG.Cards.Common.Matchmaking");
			var textHeight = font.MeasureString(text).Y;
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, text, textPos, font: font);

			if(lookingForGamePlayers.Count == 0)
			{
				int dotCount = TCGPlayer.TotalGameTime.Seconds % 4;
				text = Language.GetTextValue("Mods.TerraTCG.Cards.Common.WaitingForOpponents") + 
					string.Concat(Enumerable.Repeat(".", dotCount));
				CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, text, textPos + Vector2.UnitY * textHeight, font: font);
			}

			var headDrawPos = textPos + new Vector2(12, textHeight + 16);
			var headTexture = LookingForGamePlayerHeadRenderer.Instance.PlayerHeadRenderTarget;
			for(int i = 0; i < lookingForGamePlayers.Count; i++)
			{
				var player = lookingForGamePlayers[i];
				var headFrame = headTexture.Frame(1, MAX_OPPONENTS, 0, i);
				var origin = new Vector2(headFrame.Width, headFrame.Height) / 2;
				var effects = LookingForGamePlayerHeadRenderer.Instance.FrameEffects[i];
				spriteBatch.Draw(headTexture, headDrawPos, headFrame, Color.White, 0, origin, 1, effects, 0);

				textPos = headDrawPos + new Vector2(24, -textHeight/4);
				CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, player.Player.name, textPos, font: font);

				headDrawPos.Y += 48;
			}
        }
    }
}
