using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.GameState;
using static TerraTCG.Common.GameSystem.BotPlayer.BotDecks;

namespace TerraTCG.Common.GameSystem.PackOpening
{
	// Collection of the related pools of cards that can appear in packs
	// Packs will typically have cards from 1-2 pools
	internal class CardPools
	{
		// Cards that can appear in any pack, generally, "not good" cards
		public static CardCollection AllCards => new()
		{
			Cards = ModContent.GetContent<BaseCardTemplate>()
				.Select(t => t.Card)
				.Where(c=>c.IsCollectable && c.SubTypes[0] != CardSubtype.BOSS)
				.ToList()
		};

		public static CardCollection CommonCards => new()
		{
			Cards = [
				GetCard<BlueSlime>(), 
				GetCard<GreenSlime>(), 
				GetCard<Goldfish>(), 
				GetCard<Squirrel>(), 
				GetCard<Bunny>(), 
				GetCard<Skeleton>(), 
				GetCard<Bat>(), 
				GetCard<IronskinPotion>(), 
				GetCard<HealingPotion>(), 
				GetCard<RagePotion>(), 
				GetCard<SwiftnessPotion>(), 
				GetCard<ThornsPotion>(), 
				GetCard<ThrowingKnife>(), 
				GetCard<Shuriken>(), 
				GetCard<PoisonedKnife>(), 
				GetCard<CopperShortsword>(), 
				GetCard<FledglingWings>(), 
				GetCard<PlatinumBroadsword>(), 
				GetCard<Shackle>(), 
				GetCard<Guide>(), 
				GetCard<Dryad>(), 
				GetCard<Wizard>(), 
				GetCard<PartyGirl>(), 
				GetCard<Wizard>(), 
			]
		};

		public static CardCollection ForestCards => new()
		{
			Cards = [
				GetCard<BlueSlime>(), 
				GetCard<GreenSlime>(), 
				GetCard<EnchantedNightcrawler>(), 
				GetCard<DemonEye>(), 
				GetCard<Goldfish>(), 
				GetCard<Squirrel>(), 
				GetCard<Harpy>(), 
				GetCard<PosessedArmor>(), 
				GetCard<SlimedZombie>(), 
				GetCard<WanderingEye>(), 
				GetCard<Wraith>(), 
				GetCard<Zombie>(), 
			]
		};

		public static CardCollection CavernCards => new()
		{
			Cards = [
				GetCard<AngryBones>(), 
				GetCard<ArmoredSkeleton>(), 
				GetCard<Bat>(), 
				GetCard<Crawdad>(), 
				GetCard<GiantShelly>(), 
				GetCard<MotherSlime>(), 
				GetCard<Salamander>(), 
				GetCard<Skeleton>(), 
				GetCard<ToxicSludge>(), 
				GetCard<UndeadMiner>(), 
				GetCard<UndeadViking>(), 
			]
		};

		public static CardCollection JungleCards => new()
		{
			Cards = [
				GetCard<DoctorBones>(),
				GetCard<GiantTortoise>(),
				GetCard<Hornet>(),
				GetCard<JungleBat>(),
				GetCard<JungleTurtle>(),
				GetCard<MossHornet>(),
				GetCard<Piranha>(),
				GetCard<SpikedJungleSlime>(),
				GetCard<BladeOfGrass>(),
			]
		};

		public static CardCollection BloodMoonCards => new()
		{
			Cards = [
				GetCard<BloodZombie>(),
				GetCard<Drippler>(),
				GetCard<ViciousBunny>(),
				GetCard<ViciousGoldfish>(),
				GetCard<WanderingEyeFish>(),
				GetCard<ZombieMerman>(),
			]
		};

		public static CardCollection DungeonCards => new()
		{
			Cards = [
				GetCard<CobaltShield>(),
				GetCard<Muramasa>(),
				GetCard<AngryBones>(),
				GetCard<CursedSkull>(),
				GetCard<DarkCaster>(),
				GetCard<OldMan>(), 
			]
		};

		public static CardCollection EvilCards => new()
		{
			Cards = [
				GetCard<Crimera>(),
				GetCard<FaceMonster>(),
				GetCard<ViciousBunny>(),
				GetCard<ViciousGoldfish>(),
				GetCard<BloodButcherer>(),
				GetCard<EaterOfSouls>(),
				GetCard<Devourer>(),
			]
		};

		public static CardCollection OceanCards => new()
		{
			Cards = [
				GetCard<Crab>(),
				GetCard<Dolphin>(),
				GetCard<Jellyfish>(),
				GetCard<Shark>(),
				GetCard<FeralClaws>(),
			]
		};

		public static CardCollection GoblinCards => new()
		{
			Cards = [
				GetCard<GoblinArcher>(),
				GetCard<GoblinScout>(),
				GetCard<GoblinThief>(),
				GetCard<GoblinWarlock>(),
			]
		};

		public static CardCollection MushroomCards => new()
		{
			Cards = [
				GetCard<AnomuraFungus>(),
				GetCard<GlowingSnail>(),
				GetCard<SporeBat>(),
				GetCard<MushroomZombie>(),
				GetCard<SporeSkeleton>(),
				GetCard<HealingPotion>(),
				GetCard<Nurse>(),
			]
		};

		public static CardCollection SlimeCards => new()
		{
			Cards = [
				GetCard<BlueSlime>(),
				GetCard<GreenSlime>(),
				GetCard<Pinky>(),
				GetCard<SlimedZombie>(),
				GetCard<MotherSlime>(),
				GetCard<ToxicSludge>(),
				GetCard<SpikedJungleSlime>(),
			]
		};


		public static CardCollection QueenBeePromoCards => new()
		{
			Cards = [
				GetCard<QueenBee>(),
				GetCard<Bee>(),
			]
		};

		public static CardCollection BOCPromoCards => new()
		{
			Cards = [
				GetCard<BrainOfCthulhu>(),
				GetCard<Creeper>(),
			]
		};

		public static CardCollection EOCPromoCards => new()
		{
			Cards = [
				GetCard<EyeOfCthulhu>(),
				GetCard<ServantOfCthulhu>(),
			]
		};

		public static CardCollection EOWPromoCards => new()
		{
			Cards = [
				GetCard<EaterOfWorlds>(),
				GetCard<LightsBane>(),
			]
		};

		public static CardCollection SkeletronPromoCards => new()
		{
			Cards = [
				GetCard<Skeletron>(),
			]
		};

		public static CardCollection KingSlimePromoCards => new()
		{
			Cards = [
				GetCard<KingSlime>(),
				GetCard<SpikedSlime>(),
			]
		};

		public static CardCollection WallOfFleshPromoCards => new()
		{
			Cards = [
				GetCard<WallOfFlesh>(),
				GetCard<Leech>(),
				GetCard<TorturedSoul>(),
			]
		};
		public static CardCollection MimicPromoCards => new()
		{
			Cards = [
				GetCard<Mimic>(),
				GetCard<Tim>(),
				GetCard<LostGirl>(),
				GetCard<AngelStatue>(),
			]
		};

	}
}
