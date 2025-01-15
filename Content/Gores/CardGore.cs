using Microsoft.CodeAnalysis.Operations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Content.NPCs;

namespace TerraTCG.Content.Gores
{
	internal class CardGore : ModGore
	{
		public List<Card> AllCards { get; set; }

		public override void Load()
		{
			// TODO this is replicated many places
			AllCards = ModContent.GetContent<BaseCardTemplate>().
				Select(c => c.Card).ToList();

			base.Load();
		}
		public override void SetStaticDefaults()
		{
			ChildSafety.SafeGore[Type] = true;
		}

		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			if(source is EntitySource_Death deathSource)
			{
				CardCollection deck;
				if(deathSource.Entity is Player player)
				{
					deck = player.GetModPlayer<TCGPlayer>().Deck;
				} else if (deathSource.Entity is NPC npc && npc.boss)
				{
					deck = ModContent.GetInstance<NPCDeckMap>().NPCDecklists[npc.netID][0].DeckList;
				} else
				{
					return;
				}
				var card = Main.rand.NextFromList([.. deck.Cards]);
				// Store the card to be drawn in the gore's scale field
				gore.scale = AllCards.IndexOf(card);
			} 
			base.OnSpawn(gore, source);
		}

		public override bool Update(Gore gore)
		{
			return base.Update(gore);
		}

	}

	// Custom draw code for the active card gores - use scale to pass data
	internal class CardGoreSystem : ModSystem
	{
		public override void Load()
		{
			On_Main.DrawGore += On_Main_DrawGore;
		}

		private void On_Main_DrawGore(On_Main.orig_DrawGore orig, Main self)
		{
			orig.Invoke(self);
			if(Main.dedServ)
			{
				return;
			}

			var cardGoreType = ModContent.GoreType<CardGore>();
			for(int i = 0; i < Main.maxGore; i++)
			{
				var gore = Main.gore[i];
				if(gore.active && gore.type == cardGoreType)
				{
					DrawCardGore(Main.spriteBatch, gore);
				}
			}
		}

		private static void DrawCardGore(SpriteBatch spriteBatch, Gore gore)
		{
			var allCards = ModContent.GetInstance<CardGore>().AllCards;
			if(gore.alpha < 0 || gore.alpha >= allCards.Count)
			{
				return;
			}
			var texture = allCards[(int)gore.scale].Texture;

			var origin = new Vector2(texture.Width(), texture.Height()) / 2;
			var lightColor = Lighting.GetColor((int)gore.position.X / 16, (int)gore.position.Y / 16);
			// card textures are very large, scale them down
			var scale = 0.2f;
			spriteBatch.Draw(texture.Value, gore.position - Main.screenPosition, texture.Value.Bounds, lightColor, gore.rotation, origin, scale, SpriteEffects.None, 0f);

		}
	}
}
