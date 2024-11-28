using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;
using TerraTCG.Common.GameSystem.Drawing;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class GameFieldElement : UIElement
    {
        internal Vector2 Position => new(Left.Pixels, Top.Pixels);
        public override void Update(GameTime gameTime)
        {
            if(ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var texture = FieldRenderer.Instance.PerspectiveRenderTarget;
            if(texture != null)
            {
                spriteBatch.Draw(texture, Position, Color.White);
            }
        }
    }
}
