using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem;
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

	// Cache for NPC fields used in determining duel outcome -
	// used in case the NPC dies in multiplayer while the player is dueling it
	internal readonly struct NPCInfoCache(NPC npc)
	{
		public int NpcId { get; } = npc.netID;

		public string FullName { get; } = npc.FullName;

		public bool IsBoss { get; } = npc.boss;
	}

	internal readonly struct NamedNPCDeck(LocalizedText name, CardCollection deckList, NPCDuelReward reward, List<string> prerequisites, bool isTutorial)
    {

        public NamedNPCDeck(string nameKey, CardCollection deckList, NPCDuelReward reward = default, List<string> prerequisites = null, bool isTutorial = false) : 
            this(
				Language.GetText($"Mods.TerraTCG.Cards.DeckNames.{nameKey}"), 
				deckList, 
				reward, 
				(prerequisites ?? []).Select(p=>$"TerraTCG/Mods.TerraTCG.Cards.DeckNames.{p}").ToList(), 
				isTutorial)
        {

        }
        public LocalizedText Name { get; } = name;

		public string Mod { get; } = ModContent.GetInstance<TerraTCG>().Name;

		public string Key => $"{Mod}/{Name.Key}";
        public CardCollection DeckList { get; } = deckList;
		public NPCDuelReward Reward { get; } = reward;
		public bool IsTutorial { get; } = isTutorial;

		internal bool IsUnlocked(TCGPlayer player) =>
			prerequisites.Count == 0 || prerequisites.All(player.DefeatedDecks.Contains);
	}
    internal class NPCDeckMap : ModSystem
    {
        internal Dictionary<int, List<NamedNPCDeck>> NPCDecklists = new ()
        {
			// Duel-able Town NPCs
            [NPCID.Guide] = [
                new("Tutorial", BotDecks.GetStarterDeck(), isTutorial: true),
                new("ForestBeginner", BotDecks.GetStarterDeck(), GetReward<ForestPack>(2)),
                new("Forest", BotDecks.GetForestDeck(), GetReward<ForestPack>(3), ["ForestBeginner"])
            ],
            [NPCID.TownSlimeBlue] = [
                new("SlimeBeginner", BotDecks.GetStarterSlimeDeck(), GetReward<SlimePack>(2)),
                new("Slime", BotDecks.GetSlimeDeck(), GetReward<SlimePack>(3), ["SlimeBeginner"])
            ],
            [NPCID.WitchDoctor] = [
                new("JungleBeginner", BotDecks.GetStarterJungleDeck(), GetReward<JunglePack>(2)),
                new("Jungle", BotDecks.GetJungleDeck(), GetReward<JunglePack>(3), ["JungleBeginner"]),
            ],
            [NPCID.ArmsDealer] = [
                new("BloodMoonBeginner", BotDecks.GetStarterBloodMoonDeck(), GetReward<BloodMoonPack>(2)),
                new("BloodMoon", BotDecks.GetBloodMoonDeck(), GetReward<BloodMoonPack>(3), ["BloodMoonBeginner"]),
            ],
            [NPCID.Merchant] = [
                new("SkeletonsBeginner", BotDecks.GetStarterSkeletonDeck(), GetReward<CavernPack>(2)),
                new("Skeletons", BotDecks.GetSkeletonDeck(), GetReward<CavernPack>(3), ["SkeletonsBeginner"]),
            ],
            [NPCID.Clothier] = [
                new("Curse", BotDecks.GetCurseDeck(), GetReward<DungeonPack>(2)),
            ],
            [NPCID.TravellingMerchant] = [
                new("Treasure", BotDecks.GetMimicDeck(), GetReward<MimicPack>(2)),
            ],
            [NPCID.Nurse] = [
                new("MushroomBeginner", BotDecks.GetStarterMushroomDeck(), GetReward<MushroomPack>(2)),
                new("Mushroom", BotDecks.GetMushroomDeck(), GetReward<MushroomPack>(3), ["MushroomBeginner"]),
            ],
            [NPCID.GoblinTinkerer] = [
                new("GoblinsBeginner", BotDecks.GetStarterGoblinDeck(), GetReward<GoblinPack>(2)),
                new("Goblins", BotDecks.GetGoblinDeck(), GetReward<GoblinPack>(3), ["GoblinsBeginner"]),
            ],
            [NPCID.Angler] = [
                new("CrabsBeginner", BotDecks.GetStarterCrabDeck(), GetReward<OceanPack>(2)),
                new("Crabs", BotDecks.GetCrabDeck(), GetReward<OceanPack>(3), ["CrabsBeginner"]),
            ],
			// Bosses
			[NPCID.QueenBee] = [
				new("QueenBee", BotDecks.GetQueenBeeDeck(), GetReward<QueenBeePack>(1)),
			],
			[NPCID.KingSlime] = [
				new("KingSlime", BotDecks.GetKingSlimeDeck(), GetReward<KingSlimePack>(1)),
			],
			[NPCID.BrainofCthulhu] = [
				new("BoC", BotDecks.GetBoCDeck(), GetReward<BOCPack>(1)),
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
