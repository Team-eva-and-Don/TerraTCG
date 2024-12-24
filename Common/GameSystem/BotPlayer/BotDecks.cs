using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.CardData;
using TerraTCG.Common.GameSystem.GameState;

namespace TerraTCG.Common.GameSystem.BotPlayer
{
    internal class BotDecks
    {
        private static readonly Random random = new Random();
        public static Card CreateCard<T>() where T : ModSystem, ICardTemplate
        {
            return ModContent.GetInstance<T>().CreateCard();
        }
        
        public static CardCollection GetRandomDeck()
        {
            var allDecks = new List<Func<CardCollection>> {
                GetJungleDeck,
                GetForestDeck,
                GetBloodMoonDeck
            };

            return allDecks[Math.Abs((int)random.NextInt64()) % allDecks.Count].Invoke();
        }

        public static CardCollection GetJungleDeck() =>
            new ()
            {
                Cards = [
                    CreateCard<Bunny>(), 
                    CreateCard<Bunny>(), 
                    CreateCard<Dryad>(), 
                    CreateCard<Dryad>(), 
                    CreateCard<Goldfish>(), 
                    CreateCard<Guide>(),
                    CreateCard<Guide>(),
                    CreateCard<JungleTurtle>(), 
                    CreateCard<JungleTurtle>(), 
                    CreateCard<Tim>(),
                    CreateCard<Tim>(),
                    CreateCard<Wizard>(),
                    CreateCard<Wizard>(),
                    CreateCard<SpikedJungleSlime>(),
                    CreateCard<SpikedJungleSlime>(),
                    CreateCard<HealingPotion>(), 
                    CreateCard<HealingPotion>(), 
                    CreateCard<IronskinPotion>(), 
                    CreateCard<CobaltShield>(), 
                    CreateCard<CobaltShield>(), 
                ]
            };
        public static CardCollection GetBloodMoonDeck() =>
            new()
            {
                Cards = [
                    CreateCard<Guide>(), 
                    CreateCard<Guide>(), 
                    CreateCard<Dryad>(), 
                    CreateCard<Dryad>(), 
                    CreateCard<Wizard>(), 
                    CreateCard<Wizard>(), 
                    CreateCard<PartyGirl>(), 
                    CreateCard<SwiftnessPotion>(), 
                    CreateCard<AntlionSwarmer>(), 
                    CreateCard<AntlionSwarmer>(), 
                    CreateCard<AntlionCharger>(), 
                    CreateCard<AntlionCharger>(), 
                    CreateCard<RagePotion>(), 
                    CreateCard<RagePotion>(), 
                    CreateCard<BloodZombie>(), 
                    CreateCard<BloodZombie>(), 
                    CreateCard<Drippler>(), 
                    CreateCard<Drippler>(), 
                    CreateCard<WanderingEyeFish>(), 
                    CreateCard<WanderingEyeFish>(), 
                ]
            };
        public static CardCollection GetForestDeck() =>
            new()
            {
                Cards = [
                    CreateCard<Guide>(), 
                    CreateCard<Guide>(), 
                    CreateCard<Dryad>(), 
                    CreateCard<Wizard>(), 
                    CreateCard<Wizard>(), 
                    CreateCard<OldMan>(), 
                    CreateCard<Bunny>(), 
                    CreateCard<Bunny>(), 
                    CreateCard<Squirrel>(), 
                    CreateCard<CopperShortsword>(), 
                    CreateCard<CopperShortsword>(), 
                    CreateCard<Zombie>(), 
                    CreateCard<Zombie>(), 
                    CreateCard<Skeleton>(), 
                    CreateCard<Skeleton>(), 
                    CreateCard<DemonEye>(), 
                    CreateCard<DemonEye>(), 
                    CreateCard<HealingPotion>(), 
                    CreateCard<FledglingWings>(), 
                    CreateCard<Harpy>(), 
                ]
            };
    }
}
