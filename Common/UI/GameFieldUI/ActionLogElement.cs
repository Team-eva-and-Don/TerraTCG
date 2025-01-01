using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using TerraTCG.Common.Configs;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.GameActions;
using TerraTCG.Common.UI.Common;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class ActionLogElement : CustomClickUIElement
    {
        internal const int MAX_ACTION_LINES = 20;

        internal IEnumerable<(Turn, ActionLogInfo)> VisibleLogs { get; set; } = [];

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(!ModContent.GetInstance<ClientConfig>().ShowActionLog)
            {
                VisibleLogs = [];
                return;
            }
            var font = FontAssets.ItemStack.Value;
            var gamePlayer = TCGPlayer.LocalGamePlayer;
            VisibleLogs = gamePlayer.Game.Turns
                .Select(t => t.ActionLog.Select(al => (t, al)))
                .SelectMany(a=>a)
                .Reverse()
                .Take(MAX_ACTION_LINES);

            var boundsCheckPos = Position;
            foreach(var (turn, action) in VisibleLogs)
            {
                foreach (var actionLine in action.Message.Split('\n').Reverse())
                {
                    var bbox = font.MeasureString(actionLine);
                    var logBounds = new Rectangle((int)boundsCheckPos.X, (int)boundsCheckPos.Y, (int)bbox.X, (int)bbox.Y);
                    PlayTickIfMouseEntered(logBounds);
                    if(logBounds.Contains(Main.mouseX, Main.mouseY))
                    {
                        TCGPlayer.LocalPlayer.MouseoverCard = action.Card;
                        TCGPlayer.LocalPlayer.MouseoverZone = null;
                    }
                    boundsCheckPos.Y -= bbox.Y;
                }
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var drawPos = Position;
            var font = FontAssets.ItemStack.Value;
            var gamePlayer = TCGPlayer.LocalGamePlayer;
            foreach (var (turn, action) in VisibleLogs)
            {
                var color = turn.ActivePlayer == gamePlayer ? Color.SkyBlue : Color.Coral;
                color *= Main.mouseTextColor / 255f;
                foreach (var actionLine in action.Message.Split('\n').Reverse())
                {
                    var height = font.MeasureString(actionLine).Y;
                    CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, actionLine, drawPos, color, font: font);
                    drawPos.Y -= height;
                }
            }
        }
    }
}
