using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.Drawing.Animations;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal enum ZoneRole
    {
        OFFENSE,
        DEFENSE
    }
    internal class Zone
    {
        internal PlacedCard PlacedCard { get; set; }
        internal ZoneRole Role { get; set; }

        // TODO maybe calculate this rather than setting explicitly
        internal int Index { get; set; }

        internal IAnimation Animation { get; set; }

        internal const float CARD_DRAW_SCALE = 1f / 3f;

        public void PlaceCard(Card card)
        {
            PlacedCard = new PlacedCard(card);
        }

        public bool HasPlacedCard() => PlacedCard != null;

        private void DrawOffenseIcon(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            var texture = TextureCache.Instance.OffenseIcon;
            var frameWidth = texture.Value.Width / 4;
            var frameHeight = texture.Value.Height / 6;
            var bounds = new Rectangle(2 * frameWidth, 0, frameWidth, frameHeight);
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;
            spriteBatch.Draw(texture.Value, position, bounds, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
        }

        private void DrawDefenseIcon(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            var texture = TextureCache.Instance.DefenseIcon;
            var bounds = texture.Value.Bounds;
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;
            spriteBatch.Draw(texture.Value, position, bounds, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
        }

        internal bool IsEmpty() => PlacedCard == null;

        internal void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            var gamePlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;
            var texture = TextureCache.Instance.Zone;
            var bounds = texture.Value.Bounds;
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;
            spriteBatch.Draw(texture.Value, position + origin, bounds, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
            if(Role == ZoneRole.OFFENSE)
            {
                DrawOffenseIcon(spriteBatch, position + origin, rotation);
            } else
            {
                DrawDefenseIcon(spriteBatch, position + origin, rotation);
            }
            // Draw the placed card, either static or animated, or the empty zone icons
            if(Animation != null)
            {
                Animation.DrawZone(spriteBatch, position, rotation);
            } else if (PlacedCard != null)
            {
                texture = PlacedCard.Template.Texture;
                bounds = texture.Value.Bounds;
                origin = new Vector2(bounds.Width, bounds.Height) / 2;
                spriteBatch.Draw(texture.Value, position + origin * CARD_DRAW_SCALE, bounds, Color.White, rotation, origin, CARD_DRAW_SCALE, SpriteEffects.None, 0);
            } 

            if(gamePlayer.SelectedFieldZone == this)
            {
                texture = TextureCache.Instance.ZoneHighlighted;
                bounds = texture.Value.Bounds;
                origin = new Vector2(bounds.Width, bounds.Height) / 2;

                spriteBatch.Draw(texture.Value, position + origin, bounds, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
            } else if (gamePlayer.InProgressAction?.CanAcceptZone(this) ?? false)
            {
                texture = TextureCache.Instance.ZoneSelectable;
                bounds = texture.Value.Bounds;
                origin = new Vector2(bounds.Width, bounds.Height) / 2;

                float brightness = 0.5f + 0.5f * MathF.Sin(MathF.Tau * (float)Main._drawInterfaceGameTime.TotalGameTime.TotalSeconds / 2f);
                var color = gamePlayer.Owns(this) ? Color.LightSkyBlue : Color.LightCoral;

                spriteBatch.Draw(texture.Value, position + origin, bounds, color * brightness, rotation, origin, 1f, SpriteEffects.None, 0);
            }
        }

        internal void DrawNPC(SpriteBatch spriteBatch, Vector2 position, float scale)
        {
            var npcId = PlacedCard?.Template?.NPCID ?? 0;
            var gamePlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;
            if(npcId <= 0)
            {
                return;
            }
            if(Animation != null)
            {
                Animation.DrawZoneOverlay(spriteBatch, position, scale);
            } else
            {
                var texture = TextureCache.Instance.GetNPCTexture(npcId);
                var bounds = texture.Frame(1, Main.npcFrameCount[npcId], 0, 0);
                // position given is for point-of-contact with card, attempt to line up with
                // center-bottom of frame
                var origin = new Vector2(bounds.Width / 2, bounds.Height);
                var effects = gamePlayer.Owns(this) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(texture.Value, position, bounds, Color.White, 0, origin, scale, effects, 0);
            }
        }
    }
}
