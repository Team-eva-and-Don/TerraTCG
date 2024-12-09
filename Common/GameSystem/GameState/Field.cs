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
    internal class Field
    {
        internal List<Zone> Zones { get; set; }

        public Field(Game game)
        {
            Zones = [
                new() { Role = ZoneRole.OFFENSE, Index = 0, Game = game},
                new() { Role = ZoneRole.OFFENSE, Index = 1, Game = game},
                new() { Role = ZoneRole.OFFENSE, Index = 2, Game = game},
                new() { Role = ZoneRole.DEFENSE, Index = 3, Game = game},
                new() { Role = ZoneRole.DEFENSE, Index = 4, Game = game},
                new() { Role = ZoneRole.DEFENSE, Index = 5, Game = game},
            ];
        }

        internal void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            // Draw the front row
            for(int i = 0; i < Zones.Count / 2; i++)
            {
                var offset = position + new Vector2((FieldRenderer.CARD_WIDTH + FieldRenderer.CARD_MARGIN) * i, 0).RotatedBy(rotation);
                Zones[i].Draw(spriteBatch, offset, rotation);
            }
            // Draw the back row
            for(int i = Zones.Count/2; i < Zones.Count; i++)
            {
                var offset = position + new Vector2(
                    (FieldRenderer.CARD_WIDTH + FieldRenderer.CARD_MARGIN) * (i - Zones.Count/2), 
                    FieldRenderer.CARD_HEIGHT + FieldRenderer.CARD_MARGIN).RotatedBy(rotation);
                Zones[i].Draw(spriteBatch, offset, rotation);
            }

            // Draw the deck
            var deckPosition = position + new Vector2(
                (FieldRenderer.CARD_WIDTH + FieldRenderer.CARD_MARGIN) * (Zones.Count/2), 
                FieldRenderer.CARD_HEIGHT + FieldRenderer.CARD_MARGIN).RotatedBy(rotation);
            var texture = TextureCache.Instance.Zone;
            var bounds = texture.Value.Bounds;
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;
            spriteBatch.Draw(texture.Value, deckPosition + origin, bounds, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
        }
    }
}
