using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TerraTCG.Common.GameSystem.Drawing
{
    internal class FieldRenderer : ModSystem
    {
        public static FieldRenderer Instance => ModContent.GetInstance<FieldRenderer>();

        // TODO this probably doesn't need to be two passes
        private RenderTarget2D fieldRenderTarget;
        public RenderTarget2D PerspectiveRenderTarget { get; private set; }

        private BasicEffect effect;

        private Matrix world = Matrix.CreateTranslation(0, 0, 0);

        // Look down and slightly offset from center at the field
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, -0.5f, 1) * 2.55f, new Vector3(0, 0, 0), new Vector3(0, 1, 0));

        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1, 0.01f, 100f);

        public const int FIELD_WIDTH = 482;
        public const int FIELD_HEIGHT = 528;

        // need to run this on the main thread
        public void OnEnterWorld()
        {
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


        private void OnPreDraw(GameTime gameTime)
        {

            if(TextureCache.Field == null)
            {
                return;
            }

            Main.instance.GraphicsDevice.SetRenderTarget(PerspectiveRenderTarget);
            Main.instance.GraphicsDevice.Clear(Color.Transparent);

            effect.Texture = TextureCache.Field.Value;

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
            Main.instance.GraphicsDevice.SetRenderTarget(null);

        }
    }
}
