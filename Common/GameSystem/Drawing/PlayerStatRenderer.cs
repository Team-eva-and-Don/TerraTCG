using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.Drawing
{
    internal class PlayerStatRenderer : ModSystem
    {
        internal const int MP_PER_ROW = 4;
        public static PlayerStatRenderer Instance => ModContent.GetInstance<PlayerStatRenderer>();


        public void DrawPlayerStats(SpriteBatch spriteBatch, Vector2 position, GamePlayer player, float scale = 1f)
        {
            var statTexture = TextureCache.Instance.PlayerStatsZone.Value;
            spriteBatch.Draw(statTexture, position, statTexture.Bounds, Color.White, 0, default, scale, SpriteEffects.None, 0);

            // Health
            var hpOffset = new Vector2(15, 15);
            var hpSpacing = new Vector2(20, 0);
            var hpTexture = TextureCache.Instance.HeartIcon.Value;
            var hpOrigin = new Vector2(hpTexture.Width, hpTexture.Height) / 2;
            for(int i = 0; i < player.Health; i++)
            {
                var hpPos = position + (hpOffset + i * hpSpacing) * scale;
                spriteBatch.Draw(hpTexture, hpPos, hpTexture.Bounds, Color.White, 0, hpOrigin, scale, SpriteEffects.None, 0);
            }

            // Townsfolk Mana
            if(player.TownsfolkMana > 0)
            {
                var townsfolkOffset = new Vector2(84, 15);
                var townsolkTexture = TextureCache.Instance.TownsfolkIcon.Value;
                var townsfolkOrigin = new Vector2(townsolkTexture.Width, townsolkTexture.Height) / 2;
                var townsfolkPos = position + townsfolkOffset * scale;
                spriteBatch.Draw(townsolkTexture, townsfolkPos, townsolkTexture.Bounds, Color.White, 0, townsfolkOrigin, scale, SpriteEffects.None, 0);
            }

            // Mana
            var mpOffsets = new Vector2[] { new(17, 43), new(17, 65), new(17, 87) };
            var mpSpacing = new Vector2(22, 0);
            var mpTexture = TextureCache.Instance.ManaIcon.Value;
            var mpOrigin = new Vector2(mpTexture.Width, mpTexture.Height) / 2;

            for(int i = 0; i < player.Mana; i++)
            {
                int row = i / MP_PER_ROW;
                int col = i % MP_PER_ROW;
                var mpPos = position + (mpOffsets[row] + col * mpSpacing) * scale;
                spriteBatch.Draw(mpTexture, mpPos, mpTexture.Bounds, Color.White, 0, mpOrigin, scale, SpriteEffects.None, 0);
            }

        }
    }
}
