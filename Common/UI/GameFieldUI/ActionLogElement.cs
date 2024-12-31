using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI.Common;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class ActionLogElement : CustomClickUIElement
    {
        internal const int MAX_ACTION_LINES = 20;

        public override void Draw(SpriteBatch spriteBatch)
        {
            var drawPos = Position;
            var font = FontAssets.MouseText.Value;
            var gamePlayer = TCGPlayer.LocalGamePlayer;
            var actionTurnPairs = gamePlayer.Game.Turns
                .Select(t => t.ActionLog.Select(al => (t, al)))
                .SelectMany(a=>a)
                .Reverse()
                .Take(MAX_ACTION_LINES);
            foreach (var (turn, action) in actionTurnPairs)
            {
                var color = turn.ActivePlayer == gamePlayer ? Color.SkyBlue : Color.Coral;
                color *= Main.mouseTextColor / 255f;
                var height = font.MeasureString(action).Y;
                CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, action, drawPos, color, font: font);
                drawPos.Y -= height;
            }
        }
    }
}
