using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.PackOpening;
using TerraTCG.Content.Items;

namespace TerraTCG.Common.UI.DeckbuildUI
{
    internal class DeckbuildCardElement(Card sourceCard) : UIElement, IHasCard
    {

        public Card Card => sourceCard;
        internal int Count => TCGPlayer.LocalPlayer.DebugDeckbuildMode ? 2 :
            TCGPlayer.LocalPlayer.Collection.Cards.Where(c => c.Name == sourceCard.Name).Count();

        internal int UsedCount => TCGPlayer.LocalPlayer.Deck?.Cards
            .Where(c => c.Name == sourceCard.Name).Count() ?? 0;

        internal Vector2 Position => new(Parent.GetInnerDimensions().X + Left.Pixels, Parent.GetInnerDimensions().Y + Top.Pixels);

        internal const float CARD_SCALE = 0.8f;
        internal const int CARD_MARGIN = 8;

        internal const int CARD_HEIGHT = 180;
        internal const int CARD_WIDTH = 135;

        public override void OnInitialize()
        {
            base.OnInitialize();
            OnLeftClick += OnClickDeckbuildCard;
        }

        private void OnClickDeckbuildCard(UIMouseEvent evt, UIElement listeningElement)
        {
            var activeDeck = TCGPlayer.LocalPlayer.Deck;
            var cardCount = activeDeck.Cards.Where(c => c.Name == sourceCard.Name).Count();
            if(cardCount < Math.Min(2, Count) && activeDeck.Cards.Count < 20)
            {
                activeDeck.Cards.Add(sourceCard);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(GetBounds(out var _) && ContainsPoint(Main.MouseScreen))
            {
                TCGPlayer.LocalPlayer.MouseoverCard = sourceCard;
            }
        }

        private bool GetBounds(out Rectangle bounds)
        {
            bounds = sourceCard.Texture.Value.Bounds;
            var height = bounds.Height * CARD_SCALE;
            var parentInner = Parent.GetInnerDimensions();
            if(Position.Y > parentInner.Y + parentInner.Height)
            {
                return false;
            }
            if(Position.Y + height < parentInner.Y)
            {
                return false;
            }

            var bottomClip = parentInner.Y + parentInner.Height - (Position.Y + height);
            return bottomClip >= 0;
        }

		private string GetContainingPackName()
		{
			if(CardPools.CommonCards.Cards.Contains(Card))
			{
				return Language.GetTextValue("Mods.TerraTCG.Cards.Common.AnyPack");
			} else
			{
				return ModContent.GetContent<TerraTCGBoosterPack>().
					Where(p => p.Pack.IsPrimaryPackFor(Card))
					.Select(p =>p.DisplayName.Value)
					.FirstOrDefault();
			}

		}


		private void DrawOwnedCard(SpriteBatch spriteBatch)
		{
			FoilCardRenderer.DrawCard(spriteBatch, Card, Position, Color.White, CARD_SCALE, 0);

            var font = FontAssets.MouseText.Value;
            var countText = $"{Count - UsedCount}/{Count}";
            var countPos = Position + new Vector2(4, CARD_HEIGHT * CARD_SCALE - font.MeasureString(countText).Y + 4);
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, countText, countPos, font: font);

            if(ContainsPoint(Main.MouseScreen))
            {
                var tooltipText = 
					Language.GetTextValue("Mods.TerraTCG.Cards.Common.AddToDeck").Replace("%%", sourceCard.CardName) +
					"\n" + Language.GetTextValue("Mods.TerraTCG.Cards.Common.FoundIn") + " " + GetContainingPackName();
                DeckbuildState.SetTooltip(tooltipText);
            }
		}

		private void DrawSeenCard(SpriteBatch spriteBatch)
		{
			FoilCardRenderer.DrawCard(spriteBatch, Card, Position, Color.DimGray, CARD_SCALE, 0, details: false);

            var font = FontAssets.MouseText.Value;
            var countText = $"{Count - UsedCount}/{Count}";
            var countPos = Position + new Vector2(4, CARD_HEIGHT * CARD_SCALE - font.MeasureString(countText).Y + 4);
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, countText, countPos, color: Color.Gray, font: font);
            if(ContainsPoint(Main.MouseScreen))
            {
				var tooltipText = Language.GetTextValue("Mods.TerraTCG.Cards.Common.FoundIn") + " " + GetContainingPackName();
                DeckbuildState.SetTooltip(tooltipText);
            }
		}

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(!GetBounds(out var bounds))
            {
                return;
            }
			if(TCGPlayer.LocalPlayer.DebugDeckbuildMode || TCGPlayer.LocalPlayer.Collection.Cards.Contains(Card))
			{
				DrawOwnedCard(spriteBatch);
			} else
			{
				DrawSeenCard(spriteBatch);
			}
        }
    }
}
