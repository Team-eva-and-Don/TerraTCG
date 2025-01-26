using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations
{
    internal class QuitNotificationAnimation(TimeSpan startTime) : IFieldAnimation
    {
        public TimeSpan StartTime { get; } = startTime;

        public TimeSpan Duration { get; } = TimeSpan.FromSeconds(1f);
        public TimeSpan ElapsedTime => TCGPlayer.TotalGameTime - StartTime;

        public void DrawFieldOverlay(SpriteBatch spriteBatch, Vector2 basePosition)
        {
            var gamePlayer = TCGPlayer.LocalGamePlayer;
            var textPos = ProjectedFieldUtils.Instance.WorldSpaceToScreenSpace(gamePlayer, gamePlayer.Field.Zones[1], new(0.5f, -0.25f));
            var font = FontAssets.MouseText.Value;
			var text = Language.GetTextValue("Mods.TerraTCG.Cards.Common.PressEscAgainToExit");
			var scale = ElapsedTime < TimeSpan.FromSeconds(0.75f) ? 1 : 1 - 4 * (float)(ElapsedTime - TimeSpan.FromSeconds(0.75f)).TotalSeconds;
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, text, basePosition + textPos, color: Color.SkyBlue, scale: scale, centered: true, font: font);
        }

        public bool IsComplete() => ElapsedTime > Duration;
    }
}
