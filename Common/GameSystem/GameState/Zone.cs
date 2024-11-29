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
