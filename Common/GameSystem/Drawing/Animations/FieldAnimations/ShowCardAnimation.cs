using Microsoft.CodeAnalysis.CSharp.Syntax;
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
using TerraTCG.Common.UI.GameFieldUI;

namespace TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations
{
    internal class ShowCardAnimation(TimeSpan startTime, Card card, Zone targetZone = null, bool playerOwns = false) : IFieldAnimation
    {
        public static readonly TimeSpan DURATION = TimeSpan.FromSeconds(1.25f);

        public TimeSpan StartTime { get; } = startTime;

        public TimeSpan Duration { get; } = DURATION;
        public TimeSpan ElapsedTime => TCGPlayer.TotalGameTime - StartTime;

        public void DrawFieldOverlay(SpriteBatch spriteBatch, Vector2 basePosition)
        {
            var gamePlayer = TCGPlayer.LocalGamePlayer;
            // roughly the center
            var cardCenter = ProjectedFieldUtils.Instance.WorldSpaceToScreenSpace(gamePlayer, gamePlayer.Field.Zones[1], new(0.5f, -0.25f));

            if(ElapsedTime.TotalSeconds > 1f && targetZone != null)
            {
                float centerLerp = 4f * (float)(Duration - ElapsedTime).TotalSeconds;
                var targetCenter = ProjectedFieldUtils.Instance.WorldSpaceToScreenSpace(gamePlayer, targetZone, new(0.5f, 0.5f));
                cardCenter = Vector2.Lerp(targetCenter, cardCenter, centerLerp);
            }

            float lerpPoint;
            if (ElapsedTime.TotalSeconds < 1f)
            {
                lerpPoint = 4f * (float)ElapsedTime.TotalSeconds;
            } else
            {
                lerpPoint = 4f * CardPreviewElement.CARD_SCALE * (float)(Duration - ElapsedTime).TotalSeconds;
            }

            float scale = Math.Min(CardPreviewElement.CARD_SCALE, lerpPoint);
                
            var texture = card.Texture;

            var position = basePosition + cardCenter - new Vector2(texture.Width(), texture.Height()) / 2 * scale;

			FoilCardRenderer.DrawCard(spriteBatch, card, position, Color.White, scale, 0, playerOwns, posShift: false);
        }

        public bool IsComplete() => ElapsedTime > Duration;
    }
}
