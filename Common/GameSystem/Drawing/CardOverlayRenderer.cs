using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.Drawing
{
    delegate void DrawZoneNPC(SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects);
    internal class CardOverlayRenderer : ModSystem
    {
        public static CardOverlayRenderer Instance => ModContent.GetInstance<CardOverlayRenderer>();


		private void DrawZoneNPC(
			SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float rotation, float scale, SpriteEffects effects)
		{
            var texture = TextureCache.Instance.GetNPCTexture(card.Template.NPCID);
            var bounds = texture.Frame(1, Main.npcFrameCount[card.Template.NPCID], 0, frame);
            var origin = new Vector2(bounds.Width / 2, bounds.Height);
            spriteBatch.Draw(texture.Value, position, bounds, color ?? Color.White, rotation, origin, scale, effects, 0);
		}

        public void DefaultDrawZoneNPC(
            SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
        {
			DrawZoneNPC(spriteBatch, card, position, frame, color, 0, scale,effects);
        }

        public void DrawFlippedZoneNPC(
            SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
        {
			DrawZoneNPC(spriteBatch, card, position, frame, color, 0, scale, effects | SpriteEffects.FlipVertically);
        }
        public void DrawRotatedZoneNPC(
            SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
        {
			DrawZoneNPC(spriteBatch, card, position, frame, color, 0, scale, (effects | SpriteEffects.FlipVertically) ^ SpriteEffects.FlipHorizontally);
        }

        public DrawZoneNPC DrawSlimeNPC(float slimeScale, Color slimeColor)
        {
            return (SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects) =>
            {
                var texture = TextureCache.Instance.GetNPCTexture(card.Template.NPCID);
                var bounds = texture.Frame(1, Main.npcFrameCount[card.Template.NPCID], 0, frame);
                var origin = new Vector2(bounds.Width / 2, bounds.Height);
                spriteBatch.Draw(texture.Value, position, bounds, slimeColor, 0, origin, scale * slimeScale, effects, 0);
            };
        }

        public void DrawMimicNPC(
            SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
        {
            // only cycle through the golden mimic's frames, not all 3.
            frame = (frame % (Main.npcFrameCount[card.Template.NPCID] / 3)) + Main.npcFrameCount[card.Template.NPCID] / 3;
            DefaultDrawZoneNPC(spriteBatch, card, position, frame, color, scale, effects);
        }

        public void DrawKingSlimeNPC(
            SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
        {
            scale *= 0.5f;
            DefaultDrawZoneNPC(spriteBatch, card, position, frame, color, scale, effects);
            var npcTexture = TextureCache.Instance.GetNPCTexture(card.Template.NPCID).Value;
            var npcBounds = npcTexture.Frame(1, Main.npcFrameCount[card.Template.NPCID], 0, frame);
            var texture = TextureCache.Instance.KingSlimeCrown;
            var bounds = texture.Value.Bounds;
            var origin = new Vector2(bounds.Width / 2, bounds.Height);
            spriteBatch.Draw(texture.Value, position - Vector2.UnitY * npcBounds.Height * 0.75f * scale, bounds, color ?? Color.White, 0, origin, scale, effects, 0);
        }

        public void DrawGoblinArcherNPC(
            SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
        {
			frame = (frame % 12) + 5;
            DefaultDrawZoneNPC(spriteBatch, card, position, frame, color, scale, effects);
        }

        public void DrawQueenBeeNPC(
            SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
        {
            scale *= 0.5f;
			frame = (frame % 8) + 4;
            DefaultDrawZoneNPC(spriteBatch, card, position, frame, color, scale, effects);
        }

        public void DrawBrainOfCthulhuNPC(
            SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
        {
            scale *= 0.5f;
			frame = (frame %6) + 6;
            DefaultDrawZoneNPC(spriteBatch, card, position, frame, color, scale, effects);
        }

		internal void DrawBOCNPC(SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
		{
            scale *= 0.5f;
			frame = (frame %4) + 4;
            DefaultDrawZoneNPC(spriteBatch, card, position, frame, color, scale, effects);
		}

		internal void DrawEOCNPC(SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
		{
			// TODO we want to get the PlacedCard to do the "transform" animation
            scale *= 0.75f;
			if (card.CurrentHealth <= (card.Template.MaxHealth + 1) / 2)
			{
				frame = (frame % 3) + 3;
			} else
			{
				frame %= 3;
			}
			// TODO why do we need to shift position?
            DefaultDrawZoneNPC(spriteBatch, card, position + new Vector2(0, 48 * scale), frame, color, scale, effects | SpriteEffects.FlipVertically);
		}

		internal void DrawNoOpNPC(SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
		{
		}

		internal void DrawWOFNPC(SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
		{
            scale *= 0.75f;
			// Textures
			var textureCache = TextureCache.Instance;
			var mouthTexture = textureCache.GetNPCTexture(NPCID.WallofFlesh);
			var eyeTexture = textureCache.GetNPCTexture(NPCID.WallofFleshEye);
			var wallTexture = textureCache.WoFBack;

			// Bounds
			var mouthBounds = mouthTexture.Frame(1, Main.npcFrameCount[NPCID.WallofFlesh], 0, frame);
			var eyeBounds = eyeTexture.Frame(1, Main.npcFrameCount[NPCID.WallofFleshEye], 0, frame % Main.npcFrameCount[NPCID.WallofFleshEye]);
			var wallBounds = wallTexture.Value.Bounds;

			var yOffset = Vector2.UnitY * scale * wallBounds.Width / 16 * (effects == SpriteEffects.FlipHorizontally ? 1 : -1);
			var rotation = -MathF.PI / 2;

			// Center the bottom of the mouth on the top of the card
			position.Y -= mouthBounds.Height / 2 * scale;
			// Origins
			var mouthOrigin = new Vector2(mouthBounds.Width, mouthBounds.Height) / 2;
			var eyeOrigin = new Vector2(eyeBounds.Width, eyeBounds.Height) / 2;
			var wallOrigin = new Vector2(wallBounds.Width, wallBounds.Height) / 2;

			spriteBatch.Draw(wallTexture.Value, position + yOffset, wallBounds, color ?? Color.White, rotation, wallOrigin, scale, effects, 0);
			// spriteBatch.Draw(wallTexture.Value, position + xOffset, wallBounds, color ?? Color.White, MathF.PI/2, wallOrigin, scale, effects, 0);
			spriteBatch.Draw(mouthTexture.Value, position - yOffset, mouthBounds, color ?? Color.White, rotation, mouthOrigin, scale, effects, 0);

			for(int i = -1; i <= 1; i+=2)
			{
				var eyeOffset = new Vector2(i * scale * (wallBounds.Height - eyeBounds.Height) / 2, 0);
				spriteBatch.Draw(eyeTexture.Value, position + eyeOffset - yOffset, eyeBounds, color ?? Color.White, rotation, eyeOrigin, scale, effects, 0);
			}
		}

		public void DrawBestiaryZoneNPC(
			SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
		{
            var texture = TextureCache.Instance.GetBestiaryTexture(card.Template.NPCID);
            var bounds = texture.Frame(1, Main.npcFrameCount[card.Template.NPCID], 0, frame);
            var origin = new Vector2(bounds.Width / 2, bounds.Height);
            spriteBatch.Draw(texture.Value, position, bounds, color ?? Color.White, 0, origin, scale, effects, 0);
		}

		internal void DrawDeerclopsNPC(SpriteBatch spriteBatch, PlacedCard card, Vector2 position, int frame, Color? color, float scale, SpriteEffects effects)
		{
            scale *= 0.75f;
			frame %= 10;
            var texture = TextureCache.Instance.GetNPCTexture(card.Template.NPCID);
            var bounds = texture.Frame(5, 5, frame / 5, frame % 5);
            var origin = new Vector2(bounds.Width / 2, bounds.Height);
            spriteBatch.Draw(texture.Value, position, bounds, color ?? Color.White, 0, origin, scale, effects, 0);
		}
	}
}
