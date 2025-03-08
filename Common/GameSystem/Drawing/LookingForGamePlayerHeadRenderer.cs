using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.Netcode;
using TerraTCG.Common.UI;
using TerraTCG.Common.UI.MatchmakingUI;

namespace TerraTCG.Common.GameSystem.Drawing
{
	// Vanilla player head drawing code is rather inflexible,
	// Pre-render everything to a render target for subsequent manipulation
	// rather than diving into IL editing vanilla methods
	internal class LookingForGamePlayerHeadRenderer : ModSystem
	{
        public static LookingForGamePlayerHeadRenderer Instance => ModContent.GetInstance<LookingForGamePlayerHeadRenderer>();

        public RenderTarget2D PlayerHeadRenderTarget { get; private set; }

		public SpriteEffects[] FrameEffects { get; private set; } = new SpriteEffects[MatchmakingPanel.MAX_OPPONENTS];

        public void OnEnterWorld()
        {
            if(PlayerHeadRenderTarget != null)
            {
                return;
            }

			PlayerHeadRenderTarget = new RenderTarget2D(
				Main.graphics.GraphicsDevice,
				48,
				48 * MatchmakingPanel.MAX_OPPONENTS,
				false,
				SurfaceFormat.Color,
				DepthFormat.None,
				0,
				RenderTargetUsage.PreserveContents);
            Main.OnPreDraw += OnPreDraw;
        }

        public override void Unload()
        {
            base.Unload();
            Main.OnPreDraw -= OnPreDraw;
        }

		private static Color GetPlayerOutlineColor(int playerId, bool inMultiplayerGame)
		{
			if(!inMultiplayerGame)
			{
				return Color.White;
			}
			if(playerId == Main.myPlayer)
			{
				return TCGPlayer.LocalGamePlayer.IsMyTurn ? Color.DeepSkyBlue : Color.White;
			} else
			{
				return TCGPlayer.LocalGamePlayer.IsMyTurn ? Color.White : Color.Crimson;
			}
		}
		private void OnPreDraw(GameTime gameTime)
		{
			bool inMultiPlayerGame = (TCGPlayer.LocalGamePlayer?.Game.IsMultiplayer ?? false);
			bool shouldDraw = inMultiPlayerGame || ModContent.GetInstance<UserInterfaces>().MatchmakingLayerActive;

			if(!shouldDraw)
			{
				return;
			}

			if(MatchmakingPanel.LookingForGamePlayers.Count == 0 && !inMultiPlayerGame)
			{
				return;
			}

            Main.instance.GraphicsDevice.SetRenderTarget(PlayerHeadRenderTarget);
            Main.instance.GraphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

			Vector2 headDrawPos = new(24, 24);

			List<Player> playersToDraw = inMultiPlayerGame ?
				[Main.LocalPlayer, Main.player[(TCGPlayer.LocalGamePlayer.Opponent.Controller as NetSyncGamePlayerController).PlayerId]] :
				MatchmakingPanel.LookingForGamePlayers.Select(p => p.Player).ToList();

			for(int i = 0; i < playersToDraw.Count; i++)
			{
				var player = playersToDraw[i];
				FrameEffects[i] = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
				var xOffset = player.direction == 1 ? 0 : -8;

				var drawColor = GetPlayerOutlineColor(player.whoAmI, inMultiPlayerGame);
				Main.MapPlayerRenderer.DrawPlayerHead(Main.Camera, player, headDrawPos + Vector2.UnitX * xOffset, borderColor: drawColor);
				headDrawPos.Y += 48;
			}


            Main.spriteBatch.End();
            Main.instance.GraphicsDevice.SetRenderTarget(null);
		}
	}
}
