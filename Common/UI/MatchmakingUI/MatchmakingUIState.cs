using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;
using TerraTCG.Common.UI.GameFieldUI;

namespace TerraTCG.Common.UI.MatchmakingUI
{
	internal class MatchmakingUIState : UIState
	{
		private MatchmakingPanel matchmakingPanel;

		private int PANEL_WIDTH = 480;
		private int PANEL_HEIGHT = 270;
        const int MIN_TOP_MARGIN = 140;

        // Flag to prevent Draw() from running before element dimensions are calculated in Update()
        public bool IsOpen { get; internal set; }

		public override void OnInitialize()
		{
			base.OnInitialize();
			var bgColor = new Color(54, 53, 131, 210);
			matchmakingPanel = new()
			{
				BackgroundColor = bgColor,
				PaddingLeft = 8f,
				PaddingRight = 8f,
				PaddingTop = 8f,
				PaddingBottom = 8f,
				Width = { Pixels = PANEL_WIDTH },
				Height = { Pixels = PANEL_HEIGHT },
			};

			Append(matchmakingPanel);
		}

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(IsOpen)
            {
                base.Draw(spriteBatch);
            }
        }

		public override void Update(GameTime gameTime)
		{
			if(matchmakingPanel.Top.Pixels <= 0 || matchmakingPanel.Left.Pixels <= 0)
			{
				GameFieldState.SetRectangle(matchmakingPanel, 
					(Main.screenWidth - PANEL_WIDTH) / 2, 
					MIN_TOP_MARGIN, PANEL_WIDTH, PANEL_HEIGHT);
			}
			IsOpen = true;
			base.Update(gameTime);
		}
	}
}
