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

namespace TerraTCG.Common.GameSystem.Drawing
{
    internal class FieldRenderer : ModSystem
    {
        public static FieldRenderer Instance => ModContent.GetInstance<FieldRenderer>();

        // TODO this probably doesn't need to be two passes
        private RenderTarget2D fieldRenderTarget;
        public RenderTarget2D PerspectiveRenderTarget { get; private set; }

        // Custom render target to draw map bgs at very high scale without blur
        public RenderTarget2D MapBGRenderTarget { get; private set; }

        private BasicEffect effect;

        internal Matrix world = Matrix.CreateTranslation(0, 0, 0);

        // Look down and slightly offset from center at the field
        internal Matrix view = Matrix.CreateLookAt(new Vector3(0, -0.5f, 1) * 2.55f, new Vector3(0, 0, 0), new Vector3(0, 1, 0));

        internal Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1, 0.01f, 100f);

        public const int FIELD_MARGIN = 52; // space on all sides of the field for eg. animations
        public const int CARD_WIDTH = 90;
        public const int CARD_HEIGHT = 120;
        public const int CARD_MARGIN = 8;

        public const int FIELD_GAP = 58; // space between fields so that the player sprite(batch) isn't covered up

        public const int FIELD_WIDTH = 5 * CARD_WIDTH + 4 * CARD_MARGIN + 2 * FIELD_MARGIN;
        public const int FIELD_HEIGHT = 4 * CARD_HEIGHT + 4 * CARD_MARGIN + FIELD_GAP + 2 * FIELD_MARGIN;

        // Render the tiny map background onto a "full screen" to scale it up
        private const int LARGE_MAP_WIDTH = 1920;
        private const int LARGE_MAP_HEIGHT = 1080;


        // need to run this on the main thread, cannot find a ModSystem method
        // that guarantees it so call externally
        public void OnEnterWorld()
        {
            if(fieldRenderTarget != null)
            {
                return;
            }

            fieldRenderTarget = new RenderTarget2D(
                Main.graphics.GraphicsDevice,
                FIELD_WIDTH,
                FIELD_HEIGHT,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

            PerspectiveRenderTarget = new RenderTarget2D(
                Main.graphics.GraphicsDevice,
                FIELD_WIDTH,
                FIELD_HEIGHT,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

            MapBGRenderTarget = new RenderTarget2D(
                Main.graphics.GraphicsDevice,
                LARGE_MAP_WIDTH,
                LARGE_MAP_HEIGHT,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

            effect = new(Main.graphics.GraphicsDevice)
            {
                TextureEnabled = true,
                World = world,
                View = view,
                Projection = projection
            };

            Main.OnPreDraw += OnPreDraw;
        }

        public override void Unload()
        {
            base.Unload();
            Main.OnPreDraw -= OnPreDraw;
        }

        private void RenderFieldWithPerspective()
        {

            // 4 corners, two triangles
            Vector3[] points = [ new(-1, 1, 0), new (1, 1, 0), new (-1, -1, 0), new (1, -1, 0) ];
            VertexPositionTexture[] vertices = [
                new(points[0], new(0, 0)),
                new(points[1], new(1, 0)),
                new(points[2], new(0, 1)),
                new(points[3], new(1, 1)),
            ];
            short[] indices = [
                0, 1, 2,
                3, 2, 1,
            ];
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Main.graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    vertices,
                    0, 4,
                    indices,
                    0, 2);
            }
        }

        private void DrawGameField(GamePlayer player)
        {
            // Draw the solid background for the field
            var texture = TextureCache.Instance.Field;
            Main.spriteBatch.Draw(texture.Value, Vector2.Zero, Color.White);

            // Draw the current player's zones close to the camera
            var playerFieldPos = new Vector2(CARD_WIDTH + CARD_MARGIN + FIELD_MARGIN, FIELD_HEIGHT - 2 * CARD_HEIGHT - CARD_MARGIN - FIELD_MARGIN);
            player.Field.Draw(Main.spriteBatch, playerFieldPos, 0f);

            // Draw the opposing player's zones far from the camera
            var opponentFieldPos = new Vector2(3 * (CARD_WIDTH + CARD_MARGIN) + FIELD_MARGIN, CARD_HEIGHT + CARD_MARGIN + FIELD_MARGIN);
            player.Opponent.Field.Draw(Main.spriteBatch, opponentFieldPos, MathF.PI);
        }

        // Draw the map background that corresponds to the most populous biome in the player's active deck
        private static void DrawMapBG()
        {
			var localDeck = TCGPlayer.LocalGamePlayer?.Opponent?.Deck;
			if(localDeck == null)
			{
				return;
			}
            var dominantBiome = localDeck.Cards
                .Where(c => c.CardType == CardType.CREATURE)
                .GroupBy(c => c.SortType)
                .Select(c => (c.First(), c.Count()))
                .OrderByDescending(pair => pair.Item2)
                .Select(pair => pair.Item1.SortType)
                .FirstOrDefault();
            if(TextureCache.Instance.BiomeMapBackgrounds.TryGetValue(dominantBiome, out var texture))
            {
                Main.spriteBatch.Draw(texture.Value, new Rectangle(0, 0, LARGE_MAP_WIDTH, LARGE_MAP_HEIGHT), Color.White);
            }
        }


        private void OnPreDraw(GameTime gameTime)
        {

            var localGamePlayer = TCGPlayer.LocalGamePlayer;
            if(TextureCache.Instance.Field == null || localGamePlayer == null)
            {
                return;
            }

            // Draw the large-scale map texture
            Main.instance.GraphicsDevice.SetRenderTarget(MapBGRenderTarget);
            Main.instance.GraphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(
                SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            DrawMapBG();

            Main.spriteBatch.End();

            // Draw the field flat to a render target
            Main.instance.GraphicsDevice.SetRenderTarget(fieldRenderTarget);
            Main.instance.GraphicsDevice.Clear(Color.Transparent);

            
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            DrawGameField(localGamePlayer);

            Main.spriteBatch.End();

            // Render the field at a skewed angle via a single quad 3d primitive
            Main.instance.GraphicsDevice.SetRenderTarget(PerspectiveRenderTarget);
            Main.instance.GraphicsDevice.Clear(Color.Transparent);

            effect.Texture = fieldRenderTarget;

            RenderFieldWithPerspective();

            Main.instance.GraphicsDevice.SetRenderTarget(null);

        }
    }
}
