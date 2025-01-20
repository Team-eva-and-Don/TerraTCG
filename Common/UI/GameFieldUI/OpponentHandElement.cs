using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
using TerraTCG.Common.UI.Common;

namespace TerraTCG.Common.UI.GameFieldUI
{
    internal class OpponentHandElement : CustomClickUIElement
    {
        internal Vector2 CardPosition0 => Position - Vector2.UnitX *
            (CARD_WIDTH + CARD_MARGIN) * CARD_SCALE * TCGPlayer.LocalGamePlayer.Opponent.Hand.Cards.Count / 2;

        const float CARD_SCALE = 0.65f;
        const int CARD_MARGIN = 4;

        internal const int CARD_HEIGHT = 120;
        internal const int CARD_WIDTH = 90;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // No - op, draw-only element for the opponent's hand
        }

		private void DrawBossBehindHand(SpriteBatch spriteBatch, int bossId)
		{
			var bossCard = ModContent.GetContent<BaseCardTemplate>()
				.Select(c => c.Card)
				.Where(c => c.NPCID == bossId)
				.FirstOrDefault();
			// check the vertical clearance above the hand to see if there's enough
			// space to actually render the boss
			if (CardPosition0.Y < 64 || bossCard == null)
			{
				return;
			}
			var centerPos = new Vector2(Main.screenWidth / 2, CardPosition0.Y + CARD_HEIGHT / 2);
			var floatOffset = Vector2.UnitY * 6 * MathF.Sin(MathF.PI * (float)TCGPlayer.TotalGameTime.TotalSeconds);
			bossCard.DrawZoneNPC.Invoke(spriteBatch, bossCard, centerPos + floatOffset, 0, Color.White, 2f, SpriteEffects.None);
		}

        public override void Draw(SpriteBatch spriteBatch)
        {
            var gamePlayer = TCGPlayer.LocalGamePlayer?.Opponent;
            if (gamePlayer == null)
            {
                return;
            }
			// TODO this is hacky, but this is the background-est object in the field
			GameFieldElement.DrawMapBg(spriteBatch);

            Vector2 currentPos = CardPosition0;
			if(TCGPlayer.LocalPlayer.NPCInfo.IsBoss)
			{
				DrawBossBehindHand(spriteBatch, TCGPlayer.LocalPlayer.NPCInfo.NpcId);
			}

            var texture = TextureCache.Instance.CardBack;
            foreach (var card in gamePlayer.Hand.Cards)
            {
                // texture = card.Texture;
                spriteBatch.Draw(texture.Value, currentPos, texture.Value.Bounds, Color.White, 0, default, CARD_SCALE, SpriteEffects.None, 0f);
                currentPos.X += texture.Width() * CARD_SCALE + CARD_MARGIN;
            }
            base.Draw(spriteBatch);
        }
    }
}
