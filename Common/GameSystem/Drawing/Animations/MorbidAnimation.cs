using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.Drawing.Animations
{
	internal class MorbidAnimation : IAnimation
	{
		public TimeSpan StartTime { get ; set ; }
		public Zone SourceZone { get;  set; }

		public TimeSpan ElapsedTime => TCGPlayer.TotalGameTime - StartTime;

		public TimeSpan Duration => TimeSpan.FromSeconds(1f);

		public void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation)
		{
			// no-op
		}

		public void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
		{
			var tombstoneTexture = TextureCache.Instance.GetItemTexture(ItemID.Tombstone).Value;
			var scale = ElapsedTime < TimeSpan.FromSeconds(0.5f) ? 2f * ElapsedTime.TotalSeconds : 2f - 2f * ElapsedTime.TotalSeconds;
			var origin = new Vector2(tombstoneTexture.Width, tombstoneTexture.Height) / 2;
			spriteBatch.Draw(tombstoneTexture, basePosition, tombstoneTexture.Bounds, Color.White, 0, origin, baseScale * (float)scale, SpriteEffects.None, 0);
		}
		public void DrawZoneStats(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale)
		{
			// No stats - card is dead
		}

		public bool IsComplete() => ElapsedTime >= Duration;
	}
}
