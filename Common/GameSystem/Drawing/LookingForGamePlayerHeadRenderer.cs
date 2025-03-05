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

		private void OnPreDraw(GameTime gameTime)
		{
			if(!ModContent.GetInstance<UserInterfaces>().MatchmakingLayerActive)
			{
				return;
			}

			if(MatchmakingPanel.LookingForGamePlayers.Count == 0)
			{
				return;
			}

            Main.instance.GraphicsDevice.SetRenderTarget(PlayerHeadRenderTarget);
            Main.instance.GraphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

			Vector2 headDrawPos = new(24, 24);
			foreach(var player in MatchmakingPanel.LookingForGamePlayers)
			{
				var xOffset = player.Player.direction == 1 ? 0 : -8;
				Main.MapPlayerRenderer.DrawPlayerHead(Main.Camera, player.Player, headDrawPos + Vector2.UnitX * xOffset, borderColor: Color.White);
				headDrawPos.Y += 48;
			}


            Main.spriteBatch.End();
            Main.instance.GraphicsDevice.SetRenderTarget(null);
		}
	}
}
