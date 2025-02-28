using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.Netcode.Packets;
using TerraTCG.Common.UI.Common;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class PassTurnButton : RadialButton
    {

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var isClicked = IsClicked();
            var localPlayer = TCGPlayer.LocalPlayer;
            var gamePlayer = localPlayer.GamePlayer;
            if (!(gamePlayer?.IsMyTurn ?? false))
            {
                return;
            }

            PlayTickIfMouseEntered(Position, CLICK_RADIUS);
            if (ContainsMouse)
            {
                Main.LocalPlayer.mouseInterface = true;
                if(isClicked)
                {
                    gamePlayer.PassTurn();
					if(Main.netMode == NetmodeID.MultiplayerClient)
					{
						new PassTurnPacket(Main.LocalPlayer).Send(to: -1);
					}
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var gamePlayer = TCGPlayer.LocalGamePlayer;
            if (!(gamePlayer?.IsMyTurn ?? false))
            {
                return;
            }
            var bgTexture = ContainsMouse ? 
                TextureCache.Instance.ButtonHighlighted.Value : 
                TextureCache.Instance.Button.Value;
            var origin = new Vector2(bgTexture.Width, bgTexture.Height) / 2;
            spriteBatch.Draw(bgTexture, Position, bgTexture.Bounds, Color.White, 0, origin, 1f, SpriteEffects.None, 0);

            var buttonText = Language.GetTextValue($"Mods.TerraTCG.Cards.Common.EndTurn");
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, buttonText, Position, centered: true);
        }
    }
}
