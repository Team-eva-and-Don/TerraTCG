using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Content.Items;
using static Terraria.ModLoader.ModContent;

namespace TerraTCG
{
	internal class TerraTCGWorldGen : ModSystem
	{
		// via AmuletOfManyMinions
		private enum ChestFrame
		{
			WoodenChest = 0,
			GoldChest = 1,
			LockedGoldChest = 2,
			LockedShadowChest = 4,
			RichMahogonyChest = 8,
			IvyChest = 10,
			LivingWoodChest = 12,
			SkywareChest = 13,
			WaterChest = 17,
			MushroomChest = 32,
		}

		private readonly struct ChestLootInfo(ChestFrame chestFrame, int itemType, int frequency, int itemCount)
		{
			public ChestFrame ChestFrame { get; } = chestFrame;
			public int Frequency { get; } = frequency;
			public int ItemType { get; } = itemType;

			internal bool ShouldPlaceInChest(Chest chest)
			{
				var chestTile = Main.tile[chest.x, chest.y];
				if(chestTile.TileType == TileID.Containers)
				{
					int tileFrame = chestTile.TileFrameX / 36;
					if(tileFrame == (int)ChestFrame && Main.rand.NextBool(Frequency))
					{
						return true;
					}
				}
				return false;
			}

			internal void PlaceItemInChest(Chest chest)
			{
				for(int i = 0;  i < 40; i++)
				{
					if (chest.item[i].IsAir) 
					{
						chest.item[i].SetDefaults(ItemType);
						chest.item[i].stack = itemCount + (Main.rand.NextBool(3) ? 1 : 0);
						break;
					}
				}
			}
		}

		private readonly List<ChestLootInfo> LootTable = [
			new(ChestFrame.WoodenChest, ItemType<ForestPack>(), 3, 2),
			new(ChestFrame.GoldChest, ItemType<CavernPack>(), 3, 2),
			new(ChestFrame.GoldChest, ItemType<MimicPack>(), 8, 1),
			new(ChestFrame.MushroomChest, ItemType<MushroomPack>(), 3, 2),
			new(ChestFrame.WaterChest, ItemType<OceanPack>(), 3, 2),
			new(ChestFrame.LockedGoldChest, ItemType<DungeonPack>(), 2, 1),
			new(ChestFrame.RichMahogonyChest, ItemType<JunglePack>(), 3, 2),
			new(ChestFrame.IvyChest, ItemType<JunglePack>(), 3, 2),
		];

		public override void PostWorldGen()
		{
			base.PostWorldGen();
			for(int i = 0; i < Main.chest.Length; i++)
			{
				if (Main.chest[i] is Chest chest)
				{
					foreach(var lootEntry in LootTable)
					{
						if(lootEntry.ShouldPlaceInChest(chest))
						{
							lootEntry.PlaceItemInChest(chest);
							break;
						}
					}
				}
			}
		}
	}
}
