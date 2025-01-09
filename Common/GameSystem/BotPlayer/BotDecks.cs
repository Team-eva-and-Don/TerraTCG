using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.BotPlayer
{
    internal class BotDecks
    {
        public static Card GetCard<T>() where T : BaseCardTemplate, ICardTemplate
            => ModContent.GetInstance<T>().Card;
        
        public static CardCollection GetDeck(int deckIdx = -1)
        {
            var allDecks = new List<Func<CardCollection>> {
                GetJungleDeck,
                GetForestDeck,
                GetBloodMoonDeck,
                GetSkeletonDeck,
                GetGoblinDeck,
                GetMimicDeck,
                GetCrabDeck,
                GetMushroomDeck,
                GetCurseDeck,
                GetSlimeDeck,
            };
            var randIdx = Main.rand.Next(allDecks.Count);
            return allDecks[deckIdx == -1 ? randIdx : deckIdx].Invoke();
        }

        public static CardCollection GetStarterDeck() =>
            new()
            {
                Cards = [
                    GetCard<Guide>(), 
                    GetCard<Guide>(), 
                    GetCard<Bunny>(), 
                    GetCard<Bunny>(), 
                    GetCard<Squirrel>(), 
                    GetCard<Squirrel>(), 
                    GetCard<CopperShortsword>(), 
                    GetCard<CopperShortsword>(), 
                    GetCard<Shackle>(), 
                    GetCard<Shackle>(), 
                    GetCard<Zombie>(), 
                    GetCard<Zombie>(), 
                    GetCard<SlimedZombie>(), 
                    GetCard<SlimedZombie>(), 
                    GetCard<BlueSlime>(), 
                    GetCard<BlueSlime>(), 
                    GetCard<DemonEye>(), 
                    GetCard<DemonEye>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<HealingPotion>(), 
                ]
            };

        public static CardCollection GetStarterJungleDeck() =>
            new ()
            {
                Cards = [
                    GetCard<Bunny>(), 
                    GetCard<Bunny>(), 
                    GetCard<Dryad>(), 
                    GetCard<Goldfish>(), 
                    GetCard<Guide>(),
                    GetCard<Guide>(),
                    GetCard<JungleTurtle>(), 
                    GetCard<JungleTurtle>(), 
                    GetCard<SporeSkeleton>(),
                    GetCard<SporeSkeleton>(),
                    GetCard<Wizard>(),
                    GetCard<Piranha>(),
                    GetCard<SpikedJungleSlime>(),
                    GetCard<SpikedJungleSlime>(),
                    GetCard<HealingPotion>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<CobaltShield>(), 
                    GetCard<Shackle>(), 
                    GetCard<Skeleton>(), 
                    GetCard<Skeleton>(), 
                ]
            };

        public static CardCollection GetJungleDeck() =>
            new ()
            {
                Cards = [
                    GetCard<Bunny>(), 
                    GetCard<Bunny>(), 
                    GetCard<Dryad>(), 
                    GetCard<Goldfish>(), 
                    GetCard<Guide>(),
                    GetCard<Guide>(),
                    GetCard<JungleTurtle>(), 
                    GetCard<JungleTurtle>(), 
                    GetCard<GiantTortoise>(),
                    GetCard<GiantTortoise>(),
                    GetCard<Wizard>(),
                    GetCard<Wizard>(),
                    GetCard<Piranha>(),
                    GetCard<Piranha>(),
                    GetCard<SpikedJungleSlime>(),
                    GetCard<SpikedJungleSlime>(),
                    GetCard<HealingPotion>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<CobaltShield>(), 
                    GetCard<CobaltShield>(), 
                ]
            };

        public static CardCollection GetStarterBloodMoonDeck() =>
            new()
            {
                Cards = [
                    GetCard<Guide>(), 
                    GetCard<Guide>(), 
                    GetCard<Dryad>(), 
                    GetCard<Dryad>(), 
                    GetCard<Wizard>(), 
                    GetCard<CopperShortsword>(), 
                    GetCard<ArmsDealer>(), 
                    GetCard<SwiftnessPotion>(), 
                    GetCard<BloodZombie>(), 
                    GetCard<BloodZombie>(), 
                    GetCard<ZombieMerman>(), 
                    GetCard<ZombieMerman>(), 
                    GetCard<RagePotion>(), 
                    GetCard<RagePotion>(), 
                    GetCard<DemonEye>(), 
                    GetCard<DemonEye>(), 
                    GetCard<Drippler>(), 
                    GetCard<Drippler>(), 
                    GetCard<WanderingEyeFish>(), 
                    GetCard<WanderingEyeFish>(), 
                ]
            };

        public static CardCollection GetBloodMoonDeck() =>
            new()
            {
                Cards = [
                    GetCard<Guide>(), 
                    GetCard<Guide>(), 
                    GetCard<Dryad>(), 
                    GetCard<Dryad>(), 
                    GetCard<ArmsDealer>(), 
                    GetCard<ArmsDealer>(), 
                    GetCard<SwiftnessPotion>(), 
                    GetCard<SwiftnessPotion>(), 
                    GetCard<ViciousGoldfish>(), 
                    GetCard<ViciousGoldfish>(), 
                    GetCard<ZombieMerman>(), 
                    GetCard<ZombieMerman>(), 
                    GetCard<RagePotion>(), 
                    GetCard<RagePotion>(), 
                    GetCard<BloodZombie>(), 
                    GetCard<BloodZombie>(), 
                    GetCard<Drippler>(), 
                    GetCard<Drippler>(), 
                    GetCard<WanderingEyeFish>(), 
                    GetCard<WanderingEyeFish>(), 
                ]
            };

        public static CardCollection GetForestDeck() =>
            new()
            {
                Cards = [
                    GetCard<Guide>(), 
                    GetCard<Guide>(), 
                    GetCard<Wizard>(), 
                    GetCard<Wizard>(), 
                    GetCard<Bunny>(), 
                    GetCard<Bunny>(), 
                    GetCard<PlatinumBroadsword>(), 
                    GetCard<PlatinumBroadsword>(), 
                    GetCard<Zombie>(), 
                    GetCard<Zombie>(), 
                    GetCard<SlimedZombie>(), 
                    GetCard<SlimedZombie>(), 
                    GetCard<BlueSlime>(), 
                    GetCard<BlueSlime>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<PosessedArmor>(), 
                    GetCard<PosessedArmor>(), 
                    GetCard<Wraith>(), 
                    GetCard<Wraith>(), 
                ]
            };

        public static CardCollection GetStarterSkeletonDeck() =>
            new()
            {
                Cards = [
                    GetCard<Guide>(), 
                    GetCard<Guide>(), 
                    GetCard<Shackle>(), 
                    GetCard<Shackle>(), 
                    GetCard<FledglingWings>(), 
                    GetCard<CopperShortsword>(), 
                    GetCard<CopperShortsword>(), 
                    GetCard<PlatinumBroadsword>(), 
                    GetCard<CobaltShield>(), 
                    GetCard<CobaltShield>(), 
                    GetCard<FeralClaws>(), 
                    GetCard<AngryBones>(), 
                    GetCard<Muramasa>(), 
                    GetCard<UndeadMiner>(), 
                    GetCard<UndeadMiner>(), 
                    GetCard<Skeleton>(), 
                    GetCard<Skeleton>(), 
                    GetCard<UndeadViking>(), 
                    GetCard<UndeadViking>(), 
                    GetCard<HealingPotion>(), 
                ]
            };

        public static CardCollection GetSkeletonDeck() =>
            new()
            {
                Cards = [
                    GetCard<Guide>(), 
                    GetCard<Guide>(), 
                    GetCard<Wizard>(), 
                    GetCard<CopperShortsword>(), 
                    GetCard<FledglingWings>(), 
                    GetCard<PlatinumBroadsword>(), 
                    GetCard<PlatinumBroadsword>(), 
                    GetCard<CobaltShield>(), 
                    GetCard<CobaltShield>(), 
                    GetCard<FeralClaws>(), 
                    GetCard<FeralClaws>(), 
                    GetCard<AngryBones>(), 
                    GetCard<Muramasa>(), 
                    GetCard<UndeadMiner>(), 
                    GetCard<UndeadMiner>(), 
                    GetCard<ArmoredSkeleton>(), 
                    GetCard<ArmoredSkeleton>(), 
                    GetCard<UndeadViking>(), 
                    GetCard<UndeadViking>(), 
                    GetCard<HealingPotion>(), 
                ]
            };

        public static CardCollection GetStarterGoblinDeck() =>
            new()
            {
                Cards = [
                    GetCard<Guide>(), 
                    GetCard<Guide>(), 
                    GetCard<GoblinThief>(), 
                    GetCard<GoblinThief>(), 
                    GetCard<ThrowingKnife>(), 
                    GetCard<ThrowingKnife>(), 
                    GetCard<WanderingEyeFish>(), 
                    GetCard<WanderingEyeFish>(), 
                    GetCard<GoblinArcher>(), 
                    GetCard<GoblinArcher>(), 
                    GetCard<GoblinScout>(), 
                    GetCard<GoblinScout>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<RagePotion>(), 
                    GetCard<RagePotion>(), 
                    GetCard<Shackle>(), 
                    GetCard<IronskinPotion>(), 
                    GetCard<ThornsPotion>(), 
                    GetCard<SwiftnessPotion>(), 
                ]
            };

        public static CardCollection GetGoblinDeck() =>
            new()
            {
                Cards = [
                    GetCard<Guide>(), 
                    GetCard<Guide>(), 
                    GetCard<GoblinThief>(), 
                    GetCard<GoblinThief>(), 
                    GetCard<ThrowingKnife>(), 
                    GetCard<ThrowingKnife>(), 
                    GetCard<GoblinWarlock>(), 
                    GetCard<GoblinWarlock>(), 
                    GetCard<GoblinArcher>(), 
                    GetCard<GoblinArcher>(), 
                    GetCard<GoblinScout>(), 
                    GetCard<GoblinScout>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<RagePotion>(), 
                    GetCard<RagePotion>(), 
                    GetCard<AngelStatue>(), 
                    GetCard<AngelStatue>(), 
                    GetCard<SwiftnessPotion>(), 
                    GetCard<SwiftnessPotion>(), 
                ]
            };

        public static CardCollection GetMimicDeck() =>
            new()
            {
                Cards = [
                    GetCard<Guide>(), 
                    GetCard<Guide>(), 
                    GetCard<LostGirl>(), 
                    GetCard<LostGirl>(), 
                    GetCard<ThrowingKnife>(), 
                    GetCard<ThrowingKnife>(), 
                    GetCard<Mimic>(), 
                    GetCard<Mimic>(), 
                    GetCard<Tim>(), 
                    GetCard<Tim>(), 
                    GetCard<GoblinThief>(), 
                    GetCard<GoblinThief>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<IronskinPotion>(), 
                    GetCard<IronskinPotion>(), 
                    GetCard<AngelStatue>(), 
                    GetCard<AngelStatue>(), 
                    GetCard<SwiftnessPotion>(), 
                    GetCard<SwiftnessPotion>(), 
                ]
            };

        public static CardCollection GetStarterCrabDeck() =>
            new()
            {
                Cards = [
                    GetCard<Guide>(), 
                    GetCard<Guide>(), 
                    GetCard<Squirrel>(), 
                    GetCard<Squirrel>(), 
                    GetCard<CopperShortsword>(), 
                    GetCard<CopperShortsword>(), 
                    GetCard<ThrowingKnife>(), 
                    GetCard<RagePotion>(), 
                    GetCard<RagePotion>(), 
                    GetCard<Dolphin>(), 
                    GetCard<Dolphin>(), 
                    GetCard<Crab>(), 
                    GetCard<Crab>(), 
                    GetCard<Jellyfish>(), 
                    GetCard<Jellyfish>(), 
                    GetCard<UndeadViking>(), 
                    GetCard<UndeadViking>(), 
                    GetCard<FeralClaws>(), 
                    GetCard<FeralClaws>(), 
                    GetCard<HealingPotion>(), 
                ]
            };

        public static CardCollection GetCrabDeck() =>
            new()
            {
                Cards = [
                    GetCard<Guide>(), 
                    GetCard<Guide>(), 
                    GetCard<PartyGirl>(), 
                    GetCard<PartyGirl>(), 
                    GetCard<CopperShortsword>(), 
                    GetCard<CopperShortsword>(), 
                    GetCard<Muramasa>(), 
                    GetCard<RagePotion>(), 
                    GetCard<RagePotion>(), 
                    GetCard<Dolphin>(), 
                    GetCard<Dolphin>(), 
                    GetCard<Crab>(), 
                    GetCard<Crab>(), 
                    GetCard<Jellyfish>(), 
                    GetCard<Jellyfish>(), 
                    GetCard<Shark>(), 
                    GetCard<Shark>(), 
                    GetCard<FeralClaws>(), 
                    GetCard<FeralClaws>(), 
                    GetCard<HealingPotion>(), 
                ]
            };
        public static CardCollection GetStarterMushroomDeck() =>
            new ()
            {
                Cards = [
                    GetCard<GlowingSnail>(), 
                    GetCard<GlowingSnail>(), 
                    GetCard<Guide>(),
                    GetCard<Guide>(),
                    GetCard<MushroomZombie>(), 
                    GetCard<MushroomZombie>(), 
                    GetCard<Bunny>(),
                    GetCard<Bunny>(),
                    GetCard<Wizard>(),
                    GetCard<Wizard>(),
                    GetCard<SporeSkeleton>(),
                    GetCard<SporeSkeleton>(),
                    GetCard<SporeBat>(),
                    GetCard<SporeBat>(),
                    GetCard<HealingPotion>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<Shackle>(), 
                    GetCard<Shackle>(), 
                    GetCard<CopperShortsword>(),
                    GetCard<PlatinumBroadsword>(),
                ]
            };

        public static CardCollection GetMushroomDeck() =>
            new ()
            {
                Cards = [
                    GetCard<GlowingSnail>(), 
                    GetCard<GlowingSnail>(), 
                    GetCard<Guide>(),
                    GetCard<Guide>(),
                    GetCard<MushroomZombie>(), 
                    GetCard<MushroomZombie>(), 
                    GetCard<AnomuraFungus>(),
                    GetCard<AnomuraFungus>(),
                    GetCard<Wizard>(),
                    GetCard<Wizard>(),
                    GetCard<SporeSkeleton>(),
                    GetCard<SporeSkeleton>(),
                    GetCard<DarkCaster>(),
                    GetCard<DarkCaster>(),
                    GetCard<HealingPotion>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<CobaltShield>(), 
                    GetCard<CobaltShield>(), 
                    GetCard<PlatinumBroadsword>(),
                    GetCard<PlatinumBroadsword>(),
                ]
            };

        public static CardCollection GetCurseDeck() =>
            new ()
            {
                Cards = [
                    GetCard<GlowingSnail>(), 
                    GetCard<GlowingSnail>(), 
                    GetCard<Guide>(),
                    GetCard<Guide>(),
                    GetCard<CursedSkull>(), 
                    GetCard<ToxicSludge>(),
                    GetCard<Wizard>(),
                    GetCard<Wizard>(),
                    GetCard<DarkCaster>(),
                    GetCard<DarkCaster>(),
                    GetCard<JungleTurtle>(),
                    GetCard<JungleTurtle>(),
                    GetCard<HealingPotion>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<CobaltShield>(), 
                    GetCard<CobaltShield>(), 
                    GetCard<IronskinPotion>(),
                    GetCard<IronskinPotion>(),
                    GetCard<GiantTortoise>(),
                    GetCard<GiantTortoise>(),
                ]
            };

        public static CardCollection GetStarterSlimeDeck() =>
            new ()
            {
                Cards = [
                    GetCard<BlueSlime>(), 
                    GetCard<BlueSlime>(), 
                    GetCard<Guide>(),
                    GetCard<Guide>(),
                    GetCard<GreenSlime>(), 
                    GetCard<GreenSlime>(),
                    GetCard<Wizard>(),
                    GetCard<Wizard>(),
                    GetCard<ToxicSludge>(),
                    GetCard<ToxicSludge>(),
                    GetCard<SpikedJungleSlime>(),
                    GetCard<SpikedJungleSlime>(),
                    GetCard<HealingPotion>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<Pinky>(), 
                    GetCard<Pinky>(), 
                    GetCard<SlimedZombie>(),
                    GetCard<SlimedZombie>(),
                    GetCard<MotherSlime>(),
                    GetCard<MotherSlime>(),
                ]
            };

        public static CardCollection GetSlimeDeck() =>
            new ()
            {
                Cards = [
                    GetCard<BlueSlime>(), 
                    GetCard<BlueSlime>(), 
                    GetCard<Guide>(),
                    GetCard<Guide>(),
                    GetCard<GreenSlime>(), 
                    GetCard<GreenSlime>(),
                    GetCard<Wizard>(),
                    GetCard<Wizard>(),
                    GetCard<ToxicSludge>(),
                    GetCard<ToxicSludge>(),
                    GetCard<SpikedJungleSlime>(),
                    GetCard<SpikedJungleSlime>(),
                    GetCard<HealingPotion>(), 
                    GetCard<HealingPotion>(), 
                    GetCard<Pinky>(), 
                    GetCard<Pinky>(), 
                    GetCard<KingSlime>(),
                    GetCard<KingSlime>(),
                    GetCard<MotherSlime>(),
                    GetCard<MotherSlime>(),
                ]
            };
    }
}
