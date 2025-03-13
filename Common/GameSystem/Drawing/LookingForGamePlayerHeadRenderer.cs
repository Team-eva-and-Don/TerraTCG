using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.Netcode;
using TerraTCG.Common.UI;
using TerraTCG.Common.UI.MatchmakingUI;
using TerraTCG.Content.NPCs;

namespace TerraTCG.Common.GameSystem.Drawing
{
	// Interface wrapping the 3 things that can have a head drawn - player, Town NPC, and boss
	interface IHeadRenderer
	{
		public SpriteEffects DrawHead(Vector2 position, Color drawColor);

		public static IHeadRenderer GetRendererForGamePlayer(GamePlayer gamePlayer)
		{
			if(gamePlayer.Controller is NetSyncGamePlayerController controller)
			{
				return new PlayerHeadDrawer(Main.player[controller.PlayerId]);
			} else
			{
				return new NPCHeadDrawer(TCGPlayer.LocalPlayer.NPCInfo);
			}
	
		}
	}

	struct PlayerHeadDrawer(Player player) : IHeadRenderer
	{
		public readonly SpriteEffects DrawHead(Vector2 position, Color drawColor)
		{
			var effects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			var xOffset = player.direction == 1 ? 0 : -8;

			Main.MapPlayerRenderer.DrawPlayerHead(Main.Camera, player, position + Vector2.UnitX * xOffset, borderColor: drawColor);
			return effects;
		}
	}

	struct NPCHeadDrawer(NPCInfoCache npcInfo) : IHeadRenderer
	{
		public readonly SpriteEffects DrawHead(Vector2 position, Color drawColor)
		{
			if(npcInfo.NpcId == 0)
			{
				return SpriteEffects.None;
			}

			var localInfo = npcInfo;
			var npc = Main.npc.Where(n => n.active && n.netID == localInfo.NpcId).FirstOrDefault();
			if (npc == null)
			{
				return SpriteEffects.None;
			}
			// TODO it should be possible to set an outline color here if we do a deep enough source
			// code dive, however the public API doesn't expose an option to set it
			if(npcInfo.IsBoss)
			{
				var headIdx = npc.GetBossHeadTextureIndex();
				Main.BossNPCHeadRenderer.DrawWithOutlines(npc, headIdx, position, Color.White, 0, 1f, SpriteEffects.None);
			} else
			{
				var headIdx = TownNPCProfiles.GetHeadIndexSafe(npc);
				Main.TownNPCHeadRenderer.DrawWithOutlines(npc, headIdx, position, Color.White, 0, 1f, SpriteEffects.None);
			}
			return SpriteEffects.None;
		}
	}


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

		internal static Color GetPlayerOutlineColor(bool isMyPlayer, bool inMultiplayerGame)
		{
			if(!inMultiplayerGame)
			{
				return Color.White;
			} else if(isMyPlayer)
			{
				return TCGPlayer.LocalGamePlayer.IsMyTurn ? Color.DeepSkyBlue : Color.White;
			} else
			{
				return TCGPlayer.LocalGamePlayer.IsMyTurn ? Color.White : Color.Crimson;
			}
		}
		private void OnPreDraw(GameTime gameTime)
		{
			bool inGame = TCGPlayer.LocalGamePlayer != null;
			bool shouldDraw = inGame || ModContent.GetInstance<UserInterfaces>().MatchmakingLayerActive;

			if(!shouldDraw)
			{
				return;
			}

			if(MatchmakingPanel.LookingForGamePlayers.Count == 0 && !inGame)
			{
				return;
			}

            Main.instance.GraphicsDevice.SetRenderTarget(PlayerHeadRenderTarget);
            Main.instance.GraphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

			Vector2 headDrawPos = new(24, 24);

			List<IHeadRenderer> headsToDraw = inGame?
				[new PlayerHeadDrawer(Main.LocalPlayer), IHeadRenderer.GetRendererForGamePlayer(TCGPlayer.LocalGamePlayer.Opponent)]
				: MatchmakingPanel.LookingForGamePlayers.Select(p => new PlayerHeadDrawer(p.Player) as IHeadRenderer).ToList();

			for(int i = 0; i < headsToDraw.Count; i++)
			{
				var drawer = headsToDraw[i];
				var drawColor = GetPlayerOutlineColor(inGame && i == 0, inGame);
				FrameEffects[i] = drawer.DrawHead(headDrawPos, drawColor);
				headDrawPos.Y += 48;
			}


            Main.spriteBatch.End();
            Main.instance.GraphicsDevice.SetRenderTarget(null);
		}
	}
}
