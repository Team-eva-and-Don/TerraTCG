using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.UI.Common;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class GameFieldElement : CustomClickUIElement
    {
        internal Vector2 FieldOrigin => new (
            Position.X + FieldRenderer.FIELD_WIDTH / 2,
            Position.Y + FieldRenderer.FIELD_HEIGHT);

        internal override bool IsClicked() => !((GameFieldState)Parent).actionButtons.ContainsMouse && base.IsClicked();

        private bool PerspectiveQuadContainsMouse(ProjBounds xBounds, ProjBounds yBounds)
        {
            // TODO computing this properly outside of trail and error will be a nightmare,
            // convert from screen coordinates to projected view
            var mouseHorizontal = Main.MouseScreen.X - FieldOrigin.X;
            var mouseVertical = Main.MouseScreen.Y - FieldOrigin.Y;

            float xScale = ProjectedFieldUtils.Instance.GetScaleFactorAt(mouseVertical);
            xBounds *= xScale;

            return mouseVertical > yBounds.Min && mouseVertical < yBounds.Max &&
                   mouseHorizontal > xScale * xBounds.Min && mouseHorizontal < xScale * xBounds.Max;
        }

        public override void Update(GameTime gameTime)
        {
            var localPlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>();
            localPlayer.GameFieldOrigin = FieldOrigin;

            var gamePlayer = localPlayer.GamePlayer;
            if (gamePlayer == null || gamePlayer.Field?.Zones == null)
            {
                return;
            }
            // TODO there is probably a better place for this
            foreach (var zone in gamePlayer.Game.AllZones())
            {
                if(zone.Animation?.IsComplete() ?? false)
                {
                    zone.Animation = zone.HasPlacedCard() ?
                         new IdleAnimation(zone, gameTime.TotalGameTime) : null;
                }
            }

            // Check both players' fields
            foreach (var zone in gamePlayer.Game.AllZones())
            {
                var yBounds = ProjectedFieldUtils.Instance.GetYBoundsForZone(gamePlayer, zone);
                var xBounds = ProjectedFieldUtils.Instance.GetXBoundsForZone(gamePlayer, zone);

                if (PerspectiveQuadContainsMouse(xBounds, yBounds))
                {
                    Main.LocalPlayer.mouseInterface = true;
                    gamePlayer.MouseoverCard = zone?.PlacedCard?.Template ?? gamePlayer.MouseoverCard;
                    if(IsClicked())
                    {
                        gamePlayer.SelectZone(zone);
                        break;
                    }
                }
            }
        }

        private void DrawZoneNPCs(SpriteBatch spriteBatch)
        {
            var gamePlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;
            // Iterate backwards to layer closer zones on top of farther zones
            foreach (var zone in gamePlayer.Game.AllZones().Reverse())
            {
                var lerpPoint = gamePlayer.Owns(zone) ? 0.3f : 0.8f;
                var yPlacement = ProjectedFieldUtils.Instance.GetYBoundsForZone(gamePlayer, zone).Lerp(lerpPoint);
                var xCenter = ProjectedFieldUtils.Instance.GetXBoundsForZone(gamePlayer, zone).Center;
                var scale = ProjectedFieldUtils.Instance.GetScaleFactorAt(yPlacement);
                xCenter *= scale;
                zone.DrawNPC(spriteBatch, FieldOrigin + new Vector2(xCenter, yPlacement), scale);
            }
        }

        private void DrawPlayerStats(SpriteBatch spriteBatch)
        {
            // My player
            var gamePlayer = Main.LocalPlayer.GetModPlayer<TCGPlayer>().GamePlayer;
            var pos = FieldOrigin - new Vector2(
                FieldRenderer.FIELD_WIDTH / 2 + FieldRenderer.CARD_MARGIN, 
                TextureCache.Instance.PlayerStatsZone.Height() -
                ProjectedFieldUtils.Instance.rowHeights.Last() * FieldRenderer.FIELD_HEIGHT);
            PlayerStatRenderer.Instance.DrawPlayerStats(spriteBatch, pos, gamePlayer, 1f);

            // Opposing player
            var opponent = gamePlayer.Opponent;
            var scale = ProjectedFieldUtils.Instance.widthScaleFactors[1];
            var oppPos = FieldOrigin + new Vector2(
                3 * FieldRenderer.FIELD_WIDTH / 8 - TextureCache.Instance.PlayerStatsZone.Width() * scale, 
                ProjectedFieldUtils.Instance.rowHeights.First() * FieldRenderer.FIELD_HEIGHT);
            PlayerStatRenderer.Instance.DrawPlayerStats(spriteBatch, oppPos, opponent, scale);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var texture = FieldRenderer.Instance.PerspectiveRenderTarget;
            if(texture != null)
            {
                spriteBatch.Draw(texture, Position, Color.White);
                DrawZoneNPCs(spriteBatch);
                DrawPlayerStats(spriteBatch);
            }
        }
    }
}
