using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI.MatchmakingUI;

namespace TerraTCG.Common.GameSystem.Drawing
{
    internal class PlayerStatRenderer : ModSystem
    {
        internal const int MP_PER_ROW = 4;
        public static PlayerStatRenderer Instance => ModContent.GetInstance<PlayerStatRenderer>();

        private readonly TimeSpan Duration = TimeSpan.FromSeconds(0.2f);

        private float GetTransparency(int drawIdx, int curVal, int prevVal, float lerpPoint)
        {
            float transparency = 0;
            var checkPoint = drawIdx + 1;
            if(prevVal >= checkPoint && curVal < checkPoint)
            {
                transparency = 1 - lerpPoint;
            } else if (curVal >= checkPoint && prevVal < checkPoint)
            {
                transparency = lerpPoint;
            } else if (curVal >= checkPoint)
            {
                transparency = 1;
            }
            return transparency;
        }


        public void DrawPlayerStats(SpriteBatch spriteBatch, Vector2 position, GamePlayer player, float scale = 1f)
        {
            var statTexture = TextureCache.Instance.PlayerStatsZone.Value;
            spriteBatch.Draw(statTexture, position, statTexture.Bounds, Color.White, 0, default, scale, SpriteEffects.None, 0);
            var elapsedTime = (TCGPlayer.TotalGameTime - player.Resources.SetTime).TotalSeconds;
            float lerpPoint = MathF.Min(1, (float)(elapsedTime / Duration.TotalSeconds));

            // Health
            var hpOffset = new Vector2(15, 15);
            var hpSpacing = new Vector2(20, 0);
            var hpTexture = TextureCache.Instance.HeartIcon.Value;
            var hpOrigin = new Vector2(hpTexture.Width, hpTexture.Height) / 2;
            for(int i = 0; i < GamePlayer.MAX_HEALTH; i++)
            {
                float transparency = GetTransparency(i, player.Resources.Health, player.PrevResources.Health, lerpPoint);
                var hpPos = position + (hpOffset + i * hpSpacing) * scale;
                spriteBatch.Draw(hpTexture, hpPos, hpTexture.Bounds, Color.White * transparency, 0, hpOrigin, scale, SpriteEffects.None, 0);
            }

            // Townsfolk Mana
            {
                float transparency = GetTransparency(0, player.Resources.TownsfolkMana, player.PrevResources.TownsfolkMana, lerpPoint);
                var townsfolkOffset = new Vector2(104, 15);
                var townsolkTexture = TextureCache.Instance.TownsfolkIcon.Value;
                var townsfolkOrigin = new Vector2(townsolkTexture.Width, townsolkTexture.Height) / 2;
                var townsfolkPos = position + townsfolkOffset * scale;
                spriteBatch.Draw(townsolkTexture, townsfolkPos, townsolkTexture.Bounds, Color.White * transparency, 0, townsfolkOrigin, scale, SpriteEffects.None, 0);
            }

            // Mana
            var mpOffsets = new Vector2[] { new(27, 43), new(27, 65), new(27, 87) };
            var mpSpacing = new Vector2(22, 0);
            var mpTexture = TextureCache.Instance.ManaIcon.Value;
            var mpOrigin = new Vector2(mpTexture.Width, mpTexture.Height) / 2;

            for(int i = 0; i < GamePlayer.MAX_MANA; i++)
            {
                float transparency = GetTransparency(i, player.Resources.Mana, player.PrevResources.Mana, lerpPoint);
                if(player.ManaPerTurn >= i+1 && transparency <= 0.5f)
                {
                    transparency = 0.5f;
                }
                int row = i / MP_PER_ROW;
                int col = i % MP_PER_ROW;
                var mpPos = position + (mpOffsets[row] + col * mpSpacing) * scale;
                spriteBatch.Draw(mpTexture, mpPos, mpTexture.Bounds, Color.White * transparency, 0, mpOrigin, scale, SpriteEffects.None, 0);
            }

			// In a multiplayer game, the player's head
			if(!player.Game.IsMultiplayer)
			{
				return;
			}

			var turnTime = TCGPlayer.TotalGameTime - player.Game.CurrentTurn.StartTime;
			var scaleModifier = player.IsMyTurn ?
				1 + 1/16f * MathF.Sin(MathF.PI * (float)turnTime.TotalSeconds) : 1f;

			var headOffset = new Vector2(statTexture.Width / 2, -16) * scale;
			var headIdx = player == TCGPlayer.LocalGamePlayer ? 0 : 1;
			// var effects = player.Player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			var effects = LookingForGamePlayerHeadRenderer.Instance.FrameEffects[headIdx];
			var headTexture = LookingForGamePlayerHeadRenderer.Instance.PlayerHeadRenderTarget;
			var headFrame = headTexture.Frame(1, MatchmakingPanel.MAX_OPPONENTS, 0, headIdx);
			var origin = new Vector2(headFrame.Width, headFrame.Height) / 2;

			spriteBatch.Draw(headTexture, position + headOffset, headFrame, Color.White, 0, origin, scale * scaleModifier, effects, 0);

        }
    }
}
