using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.Drawing.Animations
{
    internal interface IAnimation
    {
        // Draw the card within the zone itself, if applicable
        void DrawZone(SpriteBatch spriteBatch, Vector2 basePosition, float rotation);

        // Draw additional items (such as an NPC sprite) on top of the zone
        void DrawZoneOverlay(SpriteBatch spriteBatch, Vector2 basePosition, float baseScale);

        bool IsComplete();
    }

    internal class AnimationUtils 
    {
        public static void DrawZoneNPC(SpriteBatch spriteBatch, Zone zone, Vector2 position, float scale, Color? color = null, int frame = 0)
        {
            var npcId = zone.PlacedCard?.Template?.NPCID ?? 0;
            if(npcId == 0)
            {
                return;
            }
            var gamePlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;

            var texture = TextureCache.Instance.GetNPCTexture(npcId);
            var bounds = texture.Frame(1, Main.npcFrameCount[npcId], 0, frame);
            var origin = new Vector2(bounds.Width / 2, bounds.Height);
            var effects = gamePlayer.Owns(zone) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture.Value, position, bounds, color ?? Color.White, 0, origin, scale, effects, 0);
        }
        public static void DrawZoneCard(SpriteBatch spriteBatch, Zone zone, Vector2 position, float rotation, Color? color = default)
        {
            var texture = zone.PlacedCard?.Template.Texture;
            if(texture == null)
            {
                return;
            }

            var bounds = texture.Value.Bounds;
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;

            spriteBatch.Draw(
                texture.Value, position + origin * Zone.CARD_DRAW_SCALE, bounds, color ?? Color.White, rotation, origin, Zone.CARD_DRAW_SCALE, SpriteEffects.None, 0);
        }
    }
}
