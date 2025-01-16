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

		private string _realTexture;
		public string RealTexture => _realTexture;

		private string _name;
		public override string Name => _name;

		// Uses blank base texture of fixed size (square) for better collision, drawn manually
		public override string Texture => $"TerraTCG/Content/Gores/{nameof(CardGore)}";

		public CardGore(string name, string realTexture)
		{
			_name = name;
			_realTexture = realTexture;
		}

		public override void SetStaticDefaults()
		{
			ChildSafety.SafeGore[Type] = true;
		}

		public override bool Update(Gore gore)
		{
			return base.Update(gore);
		}
	}

	// Custom draw code as ModGore does not have Draw hooks
	internal class CardGoreSystem : ModSystem
	{
		public override void Load()
		{
			On_Main.DrawGore += On_Main_DrawGore;
		}

		private static void On_Main_DrawGore(On_Main.orig_DrawGore orig, Main self)
		{
			orig.Invoke(self);
			if(Main.dedServ)
			{
				return;
			}

			for(int i = 0; i < Main.maxGore; i++)
			{
				var gore = Main.gore[i];
				if(gore.active && gore.ModGore is CardGore)
				{
					DrawCardGore(Main.spriteBatch, gore);
				}
			}
		}

		private static void DrawCardGore(SpriteBatch spriteBatch, Gore gore)
		{
			var texture = ModContent.Request<Texture2D>((gore.ModGore as CardGore).RealTexture).Value;

			var bounds = texture.Frame();
			var origin = bounds.Size() / 2;
			var lightColor = Lighting.GetColor((int)gore.position.X / 16, (int)gore.position.Y / 16);
			// card textures are very large, scale them down
			var scale = 0.2f;
			var opacity = (255 - gore.alpha) / 255f;
			spriteBatch.Draw(texture, gore.position - Main.screenPosition, bounds, lightColor * opacity, gore.rotation, origin, scale, SpriteEffects.None, 0f);
		}
	}
}
