using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing;

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

        public void PlaceCard(Card card)
        {
            PlacedCard = new PlacedCard(card);
        }

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

        internal void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            var localPlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>();
            if(PlacedCard != null)
            {

            } else
            {
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
            }

            if(localPlayer.GamePlayer.SelectedFieldZone == this)
            {
                var texture = TextureCache.Instance.ZoneHighlighted;
                var bounds = texture.Value.Bounds;
                var origin = new Vector2(bounds.Width, bounds.Height) / 2;

                spriteBatch.Draw(texture.Value, position + origin, bounds, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
            }
        }
    }
}
