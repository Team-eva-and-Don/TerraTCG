using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.UI.Common;
using TerraTCG.Common.UI.DeckbuildUI;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class CancelResumeGameButton : CustomClickUIElement
    {
        internal bool ContainsMouse => new Rectangle(
            (int)Left.Pixels, (int)Top.Pixels, (int)Width.Pixels, (int)Height.Pixels)
            .Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y);

        internal bool RequireDoubleClick { get; set; } = false;

        public Action OnClickAction { get; set; }

        private TimeSpan lastClickTime;
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
                    if(gameTime.TotalGameTime - lastClickTime < TimeSpan.FromSeconds(0.25) || !RequireDoubleClick)
                    {
                        OnClickAction?.Invoke();
                    }
                    lastClickTime = gameTime.TotalGameTime;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var texture = TextureCache.Instance.CancelButton.Value;
            var color = ContainsMouse ? Color.White : Color.White * 0.5f;
            spriteBatch.Draw(texture, Position, color);
            if(RequireDoubleClick && ContainsMouse)
            {
                DeckbuildState.SetTooltip(Language.GetTextValue("Mods.TerraTCG.Cards.Common.DoubleClickToClose"));
            }
        }
    }
}
