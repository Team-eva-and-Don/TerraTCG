using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
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

		// State var to hold how far back in the log the player has scrolled
		internal int SkipLines;

		// State var to check whether a new action log has been recorded and needs
		// to be immediately scrolled to
		internal int LineCount;

        private IEnumerable<(Turn, ActionLogInfo, string)> IterateLogMessageLines()
        {
            var maxLineWidth = Main.screenWidth - (int)Position.X;
            var font = FontAssets.ItemStack.Value;
            foreach(var (turn, action) in VisibleLogs)
            {
                foreach (var actionLine in action.Message.Split('\n').Reverse())
                {
                    var lines = Utils.WordwrapString(actionLine, font, maxLineWidth, 10, out var lineCount);
                    foreach (var line in lines.Where(l=>l != null).Reverse())
                    {
                        yield return (turn, action, line);
                    }
                }
            }
        }

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
                .SelectMany(a => a)
                .Reverse();

			var allLogLines = IterateLogMessageLines();
			var allLogLineCount = allLogLines.Count();

			if(allLogLineCount != LineCount)
			{
				SkipLines = 0;
			} else if(allLogLineCount > MAX_ACTION_LINES && PlayerInput.ScrollWheelDelta != 0) 
			{
				SkipLines += Math.Sign(PlayerInput.ScrollWheelDelta);
				SkipLines = Math.Min(allLogLineCount - MAX_ACTION_LINES, SkipLines);
				SkipLines = Math.Max(0, SkipLines);
			}
			LineCount = allLogLineCount;

            var boundsCheckPos = Position;
            foreach(var (turn, action, line) in allLogLines.Skip(SkipLines).Take(MAX_ACTION_LINES))
            {
                var bbox = font.MeasureString(line);
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            var drawPos = Position;
            var font = FontAssets.ItemStack.Value;
            var gamePlayer = TCGPlayer.LocalGamePlayer;
            foreach(var (turn, _, line) in IterateLogMessageLines().Skip(SkipLines).Take(MAX_ACTION_LINES))
            {
                var color = turn.ActivePlayer == gamePlayer ? Color.SkyBlue : Color.Coral;
                color *= Main.mouseTextColor / 255f;
                var height = font.MeasureString(line).Y;
                CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, line, drawPos, color, font: font);
                drawPos.Y -= height;
            }
        }
    }
}
