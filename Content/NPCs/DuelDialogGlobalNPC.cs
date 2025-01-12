using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI;
using TerraTCG.Content.Items;
using static TerraTCG.Content.NPCs.NPCDuelReward;

namespace TerraTCG.Content.NPCs
{
	internal readonly struct NPCDuelReward(int itemId, int count)
	{
		public int ItemId { get; } = itemId;
		public int Count { get; } = count;

		public static NPCDuelReward GetReward<T>(int count) where T : ModItem
			=> new(ModContent.ItemType<T>(), count);
	}

	internal readonly struct NamedNPCDeck(LocalizedText name, CardCollection deckList, NPCDuelReward reward, bool isTutorial)
    {

        public NamedNPCDeck(string nameKey, CardCollection deckList, NPCDuelReward reward = default, bool isTutorial = false) : 
            this(Language.GetText($"Mods.TerraTCG.Cards.DeckNames.{nameKey}"), deckList, reward, isTutorial)
        {

        }
        public LocalizedText Name { get; } = name;

		public string Mod { get; } = ModContent.GetInstance<TerraTCG>().Name;

		public string Key => $"{Mod}/{Name.Key}";
        public CardCollection DeckList { get; } = deckList;
		public NPCDuelReward Reward { get; } = reward;
		public bool IsTutorial { get; } = isTutorial;
    }
    internal class NPCDeckMap : ModSystem
    {
        internal Dictionary<int, List<NamedNPCDeck>> NPCDecklists = new ()
        {
            [NPCID.Guide] = [
                new("Tutorial", BotDecks.GetStarterDeck(), isTutorial: true),
                new("ForestBeginner", BotDecks.GetStarterDeck(), GetReward<ForestPack>(2)),
                new("Forest", BotDecks.GetForestDeck(), GetReward<ForestPack>(3))    
            ],
            [NPCID.TownSlimeBlue] = [
                new("SlimeBeginner", BotDecks.GetStarterSlimeDeck(), GetReward<SlimePack>(2)),
                new("Slime", BotDecks.GetSlimeDeck(), GetReward<SlimePack>(3))
            ],
            [NPCID.WitchDoctor] = [
                new("JungleBeginner", BotDecks.GetStarterJungleDeck(), GetReward<JunglePack>(2)),
                new("Jungle", BotDecks.GetJungleDeck(), GetReward<JunglePack>(3)),
            ],
            [NPCID.ArmsDealer] = [
                new("BloodMoonBeginner", BotDecks.GetStarterBloodMoonDeck(), GetReward<BloodMoonPack>(2)),
                new("BloodMoon", BotDecks.GetBloodMoonDeck(), GetReward<BloodMoonPack>(3)),
            ],
            [NPCID.Merchant] = [
                new("SkeletonsBeginner", BotDecks.GetStarterSkeletonDeck(), GetReward<CavernPack>(2)),
                new("Skeletons", BotDecks.GetSkeletonDeck(), GetReward<CavernPack>(3)),
            ],
            [NPCID.Clothier] = [
                new("Curse", BotDecks.GetCurseDeck(), GetReward<DungeonPack>(2)),
            ],
            [NPCID.TravellingMerchant] = [
                new("Treasure", BotDecks.GetMimicDeck(), GetReward<MimicPack>(2)),
            ],
            [NPCID.Nurse] = [
                new("MushroomBeginner", BotDecks.GetStarterMushroomDeck(), GetReward<MushroomPack>(2)),
                new("Mushroom", BotDecks.GetMushroomDeck(), GetReward<MushroomPack>(3)),
            ],
            [NPCID.GoblinTinkerer] = [
                new("GoblinsBeginner", BotDecks.GetStarterGoblinDeck(), GetReward<GoblinPack>(2)),
                new("Goblins", BotDecks.GetGoblinDeck(), GetReward<GoblinPack>(3)),
            ],
            [NPCID.Angler] = [
                new("CrabsBeginner", BotDecks.GetStarterCrabDeck(), GetReward<OceanPack>(2)),
                new("Crabs", BotDecks.GetCrabDeck(), GetReward<OceanPack>(3)),
            ]
        };
    }
    internal class DuelDialogGlobalNPC : GlobalNPC
    {
        public override void GetChat(NPC npc, ref string chat)
        {
            if (ModContent.GetInstance<NPCDeckMap>().NPCDecklists.ContainsKey(npc.netID))
            {
                ModContent.GetInstance<UserInterfaces>().StartNPCChat();
            }
        }
    }
}
