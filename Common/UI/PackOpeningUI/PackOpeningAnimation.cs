using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.UI.PackOpeningUI
{

    class OpeningAnimationCard
    {
        internal Card Card { get; set; }

        internal Vector2 SourcePosition { get; set; }

        internal float Angle { get; set; }

        internal TimeSpan StartTime { get; set; }

        internal TimeSpan CompleteTime { get; set; }

        internal Vector2 DestPosition => SourcePosition + new Vector2(360, 0).RotatedBy(Angle);

        internal Vector2 Position { get; set; }

        internal float FlipAmount { get; set; }

        internal float Scale { get; set; }

        private readonly TimeSpan TravelTime = TimeSpan.FromSeconds(0.25f);

        private TimeSpan WaitTime => TravelTime + TimeSpan.FromSeconds(0.25f);
        private TimeSpan FlipTime => WaitTime + TimeSpan.FromSeconds(0.25f);

        public void Update(TimeSpan elapsedTime)
        {
            // TODO more elegant way to segment animation
            elapsedTime -= StartTime;
            if(elapsedTime < TravelTime)
            {
                UpdateTravelFromPlayer(elapsedTime);
            } else if (elapsedTime < WaitTime)
            {
                UpdateIdle1(elapsedTime - TravelTime);
            } else if (elapsedTime < FlipTime)
            {
                UpdateFlip(elapsedTime - WaitTime);
            } else if (CompleteTime == default) 
            {
                UpdateIdle2(elapsedTime - FlipTime);
            } else
            {
                UpdateReturnToPlayer(elapsedTime + StartTime - CompleteTime);
            }
        }

        public bool IsRevealed(TimeSpan elapsedTime)
            => elapsedTime - StartTime >= FlipTime;

        private void UpdateTravelFromPlayer(TimeSpan elapsedTime)
        {
            var lerpPoint = MathF.Sin(MathF.Tau * (float)elapsedTime.TotalSeconds);
            Position = Vector2.Lerp(SourcePosition, DestPosition, lerpPoint);
            Scale = lerpPoint;
            FlipAmount = -1;
        }

        private void UpdateReturnToPlayer(TimeSpan elapsedTime)
        {
            var lerpPoint = Math.Max(0, 1 - 4 * (float)elapsedTime.TotalSeconds);
            Position = Vector2.Lerp(SourcePosition, DestPosition, lerpPoint);
            Scale = lerpPoint;
            FlipAmount = 1;
        }

        private void UpdateIdle1(TimeSpan elapsedTime)
        {
            var lerpPoint = 1 + 0.1f * MathF.Sin(MathF.Tau * (float)elapsedTime.TotalSeconds);
            Position = Vector2.Lerp(SourcePosition, DestPosition, lerpPoint);
            Scale = 1f;
            FlipAmount = -1;
        }

        private void UpdateIdle2(TimeSpan elapsedTime)
        {
            var lerpPoint = 1 - 0.02f * MathF.Sin(MathF.PI * (float)elapsedTime.TotalSeconds);
            Position = Vector2.Lerp(SourcePosition, DestPosition, lerpPoint);
            Scale = 1f;
            FlipAmount = 1;
        }

        private void UpdateFlip(TimeSpan elapsedTime)
        {
            var passedSeconds = (float)elapsedTime.TotalSeconds;
            var posLerpPoint = 1 + 0.1f * MathF.Sin(MathF.PI /2 + MathF.Tau * passedSeconds);
            Position = Vector2.Lerp(SourcePosition, DestPosition, posLerpPoint);
            Scale = 1f + 0.25f * Math.Max(0, MathF.Sin(MathF.Tau * 2 * passedSeconds));
            FlipAmount = Math.Min(1, -1f + 8f * (float)elapsedTime.TotalSeconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            bool isBackwards = FlipAmount < 0;
            var texture = isBackwards ? TextureCache.Instance.CardBack.Value : 
                CardWithTextRenderer.Instance.CardWithTextRenderTarget;
            var scale = isBackwards ? Scale * 2f : Scale;
            var bounds = isBackwards ? texture.Bounds : CardWithTextRenderer.Instance.CardBounds(Card);
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;

            var scaleVector = new Vector2(scale * Math.Abs(FlipAmount), scale);

            spriteBatch.Draw(texture, Position, bounds, Color.White, 0, origin, scaleVector, SpriteEffects.None, 0);
        }
    }

    internal class PackOpeningAnimation : UIElement
    {
        internal TimeSpan StartTime { get; set; }
        private TimeSpan ElapsedTime => TCGPlayer.TotalGameTime - StartTime;
        private List<OpeningAnimationCard> AnimationCards { get; set; } = [];

        private List<OpeningAnimationCard> ActiveCards => AnimationCards.Where(c => c.StartTime <= ElapsedTime).ToList();

        public override void OnInitialize()
        {
        }

        public void Reset()
        {
            AnimationCards = [ ];
            var cards = CardWithTextRenderer.Instance.ToRender;
            for(int i = 0; i < cards.Count; i++)
            {
                float angle = -MathF.PI + MathF.PI * (i + 1)  / (cards.Count + 1);
                float startTime = 0.67f * i;
                AnimationCards.Add(new OpeningAnimationCard
                {
                    Angle = angle,
                    StartTime =  TimeSpan.FromSeconds(startTime),
                    Card = cards[i]
                });
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach(var card in ActiveCards)
            {
                card.SourcePosition = new Vector2(Left.Pixels, Top.Pixels);
                card.Update(ElapsedTime);
            }
            
            if(Main.mouseLeft && AnimationCards.All(c=>c.IsRevealed(ElapsedTime)))
            {
                foreach(var card in AnimationCards)
                {
                    card.CompleteTime = ElapsedTime;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            foreach (var card in ActiveCards)
            {
                card.Draw(spriteBatch);
            }
        }
    }
}
