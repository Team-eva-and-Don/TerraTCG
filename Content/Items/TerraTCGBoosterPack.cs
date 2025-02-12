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
    internal abstract class TerraTCGBoosterPack : ModItem
    {
		internal abstract Pack Pack { get; }
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
		internal override Pack Pack => PackDefinitions.ForestPack;
	}

	internal class CavernPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.CavernPack;
	}

	internal class DungeonPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.DungeonPack;
	}

	internal class MimicPack : TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.MimicPack;
	}

	internal class EvilPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.EvilPack;
	}
	internal class GoblinPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.GoblinPack;
	}
	internal class JunglePack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.JunglePack;
	}
	internal class BloodMoonPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.BloodMoonPack;
	}

	internal class KingSlimePack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.KingSlimePack;
	}

	internal class BOCPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.BOCPack;
	}

	internal class EOCPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.EOCPack;
	}

	internal class MushroomPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.MushroomPack;
	}

	internal class OceanPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.OceanPack;
	}
	internal class QueenBeePack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.QueenBeePack;
	}
	internal class EOWPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.EOWPack;
	}
	internal class SkeletronPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.SkeletronPack;
	}

	internal class WOFPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.WOFPack;
	}

	internal class DeerclopsPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.DeerclopsPack;
	}

	internal class SlimePack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.SlimePack;
	}

	internal class SnowPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.SnowPack;
	}

	internal class HallowedPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.HallowedPack;
	}

	internal class BatPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.BatPack;
	}

	internal class CritterPack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.CritterPack;
	}

	internal class QueenSlimePack: TerraTCGBoosterPack
	{
		internal override Pack Pack => PackDefinitions.QueenSlimePack;
	}
}
