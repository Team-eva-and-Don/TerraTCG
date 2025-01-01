using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.GameState
{
    internal enum ZoneRole
    {
        OFFENSE,
        DEFENSE,
        SKILL
    }
    internal class Zone
    {
        internal CardGame Game { get; set; }
        internal PlacedCard PlacedCard { get; set; }
        internal ZoneRole Role { get; set; }

        internal int Index { get; set; }

        private List<IAnimation> animationQueue = [];
        internal IAnimation Animation { get => animationQueue.Count > 0 ? animationQueue[0] : null; set { QueueAnimation(value); } }

        internal const float CARD_DRAW_SCALE = 2f / 3f;


        public void PlaceCard(Card card)
        {
            PlacedCard = new PlacedCard(card)
            {
                IsExerted = true,
                PlaceTime = TCGPlayer.TotalGameTime,
                CardModifiers = [.. card.Modifiers?.Invoke() ?? []]
            };

            foreach (var modifier in PlacedCard.CardModifiers.Concat(Owner.Field.CardModifiers))
            {
                modifier.ModifyCardEntrance(this);
            }
        }

        public void PromoteCard(Card newCard)
        {
            var leavingCard = PlacedCard;
            var dmgTaken = leavingCard.Template.MaxHealth - leavingCard.CurrentHealth;
            // Keep all item-sourced modifiers on the card post-promotion,
            // Remove debuffs
            var itemModifiers = leavingCard.CardModifiers
                .Where(m => m.Source == CardSubtype.EQUIPMENT || m.Source == CardSubtype.CONSUMABLE)
                .ToList();
            PlaceCard(newCard);
            PlacedCard.IsExerted = false;
            PlacedCard.CurrentHealth -= dmgTaken;
            PlacedCard.AddModifiers(itemModifiers);

            foreach (var modifier in PlacedCard.CardModifiers.Concat(Owner.Field.CardModifiers))
            {
                modifier.ModifyCardEntrance(this);
            }

            QueueAnimation(new RemoveCardAnimation(leavingCard));
            QueueAnimation(new PlaceCardAnimation(PlacedCard));
            GameSounds.PlaySound(GameAction.PROMOTE_CARD);
        }

        public bool HasPlacedCard() => PlacedCard != null;

        // For defense zones, check whether an enemy is in the aligned offense zone
        public bool IsBlocked() => Role == ZoneRole.DEFENSE && !Owner.Field.Zones[Index - 3].IsEmpty();

        internal void QueueAnimation(IAnimation animation)
        {
            // Clear out any infinite idle animations
            if(!animation.IsDefault())
            {
                animationQueue = animationQueue.Where(q => !q.IsDefault()).ToList();
            }
            animation.StartTime = TCGPlayer.TotalGameTime;
            animation.SourceZone = this;
            animationQueue.Add(animation);
        }

        internal void UpdateAnimationQueue()
        {
            // Ensure that there's always an empty animation waiting at the end of the queue
            if(!IsEmpty() && (animationQueue.Count == 0 || !animationQueue.Last().IsDefault()))
            {
                QueueAnimation(new IdleAnimation(PlacedCard));
            }
            if (animationQueue.Count > 0 && animationQueue[0].IsComplete())
            {
                animationQueue.RemoveAt(0);
                if(animationQueue.Count > 0)
                {
                    animationQueue[0].StartTime = TCGPlayer.TotalGameTime;
                }
            }
        }

        public GamePlayer Owner => Game.GamePlayers.Where(p => p.Field.Zones.Contains(this)).FirstOrDefault();

        public List<Zone> Siblings => Owner.Field.Zones;

        // Helper to get the name of the card that is (or isn't) placed in the zone
        public string CardName => PlacedCard?.Template.CardName;

        private void DrawOffenseIcon(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            var texture = TextureCache.Instance.OffenseIcon;
            var frameWidth = texture.Value.Width / 4;
            var frameHeight = texture.Value.Height / 6;
            var bounds = new Rectangle(2 * frameWidth, 0, frameWidth, frameHeight);
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;
            spriteBatch.Draw(texture.Value, position, bounds, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
        }

        private void DrawDefenseIcon(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            var texture = TextureCache.Instance.DefenseIcon;
            var bounds = texture.Value.Bounds;
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;
            spriteBatch.Draw(texture.Value, position, bounds, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
        }

        internal bool IsEmpty() => PlacedCard == null;

        internal void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation)
        {
            var gamePlayer = TCGPlayer.LocalGamePlayer;
            var texture = TextureCache.Instance.Zone;
            var bounds = texture.Value.Bounds;
            var origin = new Vector2(bounds.Width, bounds.Height) / 2;
            spriteBatch.Draw(texture.Value, position + origin, bounds, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
            if(Role == ZoneRole.OFFENSE)
            {
                DrawOffenseIcon(spriteBatch, position + origin, rotation);
            } else
            {
                DrawDefenseIcon(spriteBatch, position + origin, rotation);
            }

            // Draw the placed card
            Animation?.DrawZone(spriteBatch, position, rotation);

            if(gamePlayer.SelectedFieldZone == this)
            {
                texture = TextureCache.Instance.ZoneHighlighted;
                bounds = texture.Value.Bounds;
                origin = new Vector2(bounds.Width, bounds.Height) / 2;

                spriteBatch.Draw(texture.Value, position + origin, bounds, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
            } else if (gamePlayer.InProgressAction?.CanAcceptZone(this) ?? false)
            {
                texture = TextureCache.Instance.ZoneSelectable;
                bounds = texture.Value.Bounds;
                origin = new Vector2(bounds.Width, bounds.Height) / 2;

                float brightness = 0.5f + 0.5f * MathF.Sin(MathF.Tau * (float)TCGPlayer.TotalGameTime.TotalSeconds / 2f);
                var color = gamePlayer.InProgressAction.HighlightColor(this);

                spriteBatch.Draw(texture.Value, position + origin, bounds, color * brightness, rotation, origin, 1f, SpriteEffects.None, 0);
            }
        }

        internal void DrawNPC(SpriteBatch spriteBatch, Vector2 position, float scale)
        {
            Animation?.DrawZoneOverlay(spriteBatch, position, scale);
        }
    }
}
