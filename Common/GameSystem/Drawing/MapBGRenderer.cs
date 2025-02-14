using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.GameState;
using static TerraTCG.Common.GameSystem.BotPlayer.BotDecks;

namespace TerraTCG.Common.GameSystem.Drawing
{

	// Util class for the (unfortunately not straightforward) logic of
	// choosing which Map BG to use for the given opponent
	internal class MapBGRenderer : ModSystem
	{
		private readonly struct BiomeBGInfo(Asset<Texture2D> background, int count= 0, bool skyColored = false)
		{
			public Asset<Texture2D> Background { get; } = background;
			public int Count { get; } = count;
			public bool SkyColored { get; } = skyColored;
		}

		// Render the tiny map background onto a "full screen" to scale it up
		internal const int LARGE_MAP_WIDTH = 1920;
        internal const int LARGE_MAP_HEIGHT = 1080;

		internal List<Card> CrimsonCards = [GetCard<Crimera>(), GetCard<FaceMonster>(), GetCard<Creeper>(), GetCard<BrainOfCthulhu>()];
		internal List<Card> CorruptionCards = [GetCard<EaterOfSouls>(), GetCard<Devourer>(), GetCard<EaterOfWorlds>()];

		internal List<CardSubtype> SkyColored = [
			CardSubtype.FOREST, CardSubtype.GOBLIN_ARMY, CardSubtype.JUNGLE, CardSubtype.OCEAN, CardSubtype.SNOW, CardSubtype.HALLOWED,
		];

		private static BiomeBGInfo? GetDominantBiome(CardCollection deck)
		{
			var bgMap = TextureCache.Instance.BiomeMapBackgrounds;
			var skyColorList = ModContent.GetInstance<MapBGRenderer>().SkyColored;
			var dominantBiome = deck.Cards
				.Where(c => c.CardType == CardType.CREATURE)
				.Where(c => bgMap.ContainsKey(c.SortType))
				.GroupBy(c => c.SortType)
				.Select(c => (c.First(), c.Count()))
				.OrderByDescending(pair => pair.Item2)
				.Select(pair => new BiomeBGInfo(bgMap[pair.Item1.SortType], pair.Item2, skyColorList.Contains(pair.Item1.SortType)))
				.FirstOrDefault();
			return dominantBiome;
		}

		private BiomeBGInfo? GetSpecialBG(CardCollection deck, int regularBiomeCount)
		{
			var specialBGs = TextureCache.Instance.OtherMapBackgrounds;
			foreach (var card in new Card[] {GetCard<Skeletron>(), GetCard<QueenBee>(), GetCard<WallOfFlesh>(), GetCard<SkeletronPrime>()})
			{
				if(deck.Cards.Contains(card))
				{
					return new(specialBGs[card.Name]);
				}
			}
			if(deck.Cards.Count(CrimsonCards.Contains) > regularBiomeCount)
			{
				return new(specialBGs["CRIMSON"], 0, true);
			} else if(deck.Cards.Count(CorruptionCards.Contains) > regularBiomeCount)
			{
				return new(specialBGs["CORRUPTION"], 0, true);
			}
			return null;
		}

        // Draw the map background that corresponds to the most populous biome in the player's active deck
        internal static void DrawMapBG(SpriteBatch spriteBatch)
        {
			var localDeck = TCGPlayer.LocalGamePlayer?.Opponent?.Controller.Deck;
			if(localDeck == null)
			{
				return;
			}

			var dominantBiome = GetDominantBiome(localDeck);

			var biomeToDraw = ModContent.GetInstance<MapBGRenderer>().GetSpecialBG(localDeck, dominantBiome?.Count ?? 0) ?? 
				dominantBiome ??
				new BiomeBGInfo(TextureCache.Instance.BiomeMapBackgrounds[CardSubtype.FOREST], 0, true);

			spriteBatch.Draw(
				biomeToDraw.Background.Value, 
				new Rectangle(0, 0, LARGE_MAP_WIDTH, LARGE_MAP_HEIGHT), 
				biomeToDraw.SkyColored ? Main.ColorOfTheSkies : Color.White);
        }
	}
}
