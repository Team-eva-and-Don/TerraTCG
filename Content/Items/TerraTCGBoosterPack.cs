using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem;
using TerraTCG.Common.GameSystem.PackOpening;
using TerraTCG.Common.UI;

namespace TerraTCG.Content.Items
{
	public abstract class TerraTCGBoosterPack : ModItem
    {
		public abstract Pack Pack { get; }
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 15;
            Item.useAnimation = 15;
			Item.width = 32;
			Item.height = 32;
            Item.autoReuse = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 10);
            Item.UseSound = SoundID.Item156;
        }

        public override bool CanUseItem(Player player)
            => player.whoAmI == Main.myPlayer &&
            !ModContent.GetInstance<UserInterfaces>().IsPackOpening();

        public override bool? UseItem(Player player)
        {
            if(player.whoAmI == Main.myPlayer && !ModContent.GetInstance<UserInterfaces>().IsPackOpening())
            {
				Pack.OpenPack(TCGPlayer.LocalPlayer);
                return true;
            }
            return false;
        }
    }

	internal class ForestPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.ForestPack;
	}

	internal class CavernPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.CavernPack;
	}

	internal class DungeonPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.DungeonPack;
	}

	internal class MimicPack : TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.MimicPack;
	}

	internal class EvilPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.EvilPack;
	}
	internal class GoblinPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.GoblinPack;
	}
	internal class JunglePack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.JunglePack;
	}
	internal class BloodMoonPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.BloodMoonPack;
	}

	internal class KingSlimePack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.KingSlimePack;
	}

	internal class BOCPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.BOCPack;
	}

	internal class EOCPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.EOCPack;
	}

	internal class MushroomPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.MushroomPack;
	}

	internal class OceanPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.OceanPack;
	}
	internal class QueenBeePack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.QueenBeePack;
	}
	internal class EOWPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.EOWPack;
	}
	internal class SkeletronPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.SkeletronPack;
	}

	internal class WOFPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.WOFPack;
	}

	internal class DeerclopsPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.DeerclopsPack;
	}

	internal class SlimePack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.SlimePack;
	}

	internal class SnowPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.SnowPack;
	}

	internal class HallowedPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.HallowedPack;
	}

	internal class BatPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.BatPack;
	}

	internal class CritterPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.CritterPack;
	}

	internal class QueenSlimePack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.QueenSlimePack;
	}
	internal class SkeletronPrimePack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.SkeletronPrimePack;
	}

	internal class DestroyerPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.DestroyerPack;
	}

	internal class TwinsPack: TerraTCGBoosterPack
	{
		public override Pack Pack => PackDefinitions.TwinsPack;
	}
}
