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

namespace TerraTCG.Content.NPCs
{
    internal struct NamedNPCDeck(LocalizedText name, CardCollection deckList, bool isTutorial)
    {

        public NamedNPCDeck(string nameKey, CardCollection deckList, bool isTutorial = false) : 
            this(Language.GetText($"Mods.TerraTCG.Cards.DeckNames.{nameKey}"), deckList, isTutorial)
        {

        }
        public LocalizedText Name { get; } = name;
        public CardCollection DeckList { get; } = deckList;
        public bool IsTutorial { get; } = isTutorial;
    }
    internal class NPCDeckMap : ModSystem
    {
        internal Dictionary<int, List<NamedNPCDeck>> NPCDecklists = new ()
        {
            [NPCID.Guide] = [
                new("Tutorial", BotDecks.GetStarterDeck(), isTutorial: true),
                new("ForestBeginner", BotDecks.GetStarterDeck()),
                new("Forest", BotDecks.GetForestDeck())    
            ],
            [NPCID.TownSlimeBlue] = [
                new("SlimeBeginner", BotDecks.GetStarterSlimeDeck()),
                new("Slime", BotDecks.GetSlimeDeck())
            ],
            [NPCID.WitchDoctor] = [
                new("JungleBeginner", BotDecks.GetStarterJungleDeck()),
                new("Jungle", BotDecks.GetJungleDeck()),
            ],
            [NPCID.ArmsDealer] = [
                new("BloodMoonBeginner", BotDecks.GetStarterBloodMoonDeck()),
                new("BloodMoon", BotDecks.GetBloodMoonDeck()),
            ],
            [NPCID.Merchant] = [
                new("SkeletonsBeginner", BotDecks.GetStarterSkeletonDeck()),
                new("Skeletons", BotDecks.GetSkeletonDeck()),
            ],
            [NPCID.Clothier] = [
                new("Curse", BotDecks.GetCurseDeck()),
            ],
            [NPCID.TravellingMerchant] = [
                new("Treasure", BotDecks.GetMimicDeck()),
            ],
            [NPCID.Nurse] = [
                new("MushroomBeginner", BotDecks.GetStarterMushroomDeck()),
                new("Mushroom", BotDecks.GetMushroomDeck()),
            ],
            [NPCID.GoblinTinkerer] = [
                new("GoblinsBeginner", BotDecks.GetStarterGoblinDeck()),
                new("Goblins", BotDecks.GetGoblinDeck()),
            ],
            [NPCID.Angler] = [
                new("CrabsBeginner", BotDecks.GetStarterCrabDeck()),
                new("Crabs", BotDecks.GetCrabDeck()),
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
