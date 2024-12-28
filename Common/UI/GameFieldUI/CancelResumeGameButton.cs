using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.UI.Common;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class CancelResumeGameButton : CustomClickUIElement
    {
        internal bool ContainsMouse => new Rectangle(
            (int)Left.Pixels, (int)Top.Pixels, (int)Width.Pixels, (int)Height.Pixels)
            .Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y);

        public Action OnClickAction { get; set; }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var isClicked = IsClicked();
            var localPlayer = TCGPlayer.LocalPlayer;
            var gamePlayer = localPlayer.GamePlayer;

            if (ContainsMouse)
            {
                Main.LocalPlayer.mouseInterface = true;
                if(isClicked)
                {
                    OnClickAction?.Invoke();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var texture = TextureCache.Instance.CancelButton.Value;
            var color = ContainsMouse ? Color.White : Color.White * 0.5f;
            spriteBatch.Draw(texture, Position, color);
        }
    }
}
