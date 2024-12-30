using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.BotPlayer;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.UI;

namespace TerraTCG.Content.NPCs
{
    internal class NPCDeckMap : ModSystem
    {
        internal Dictionary<int, CardCollection> NPCDecklists = new ()
        {
            [NPCID.Guide] = BotDecks.GetStarterDeck(),
            [NPCID.TownSlimeBlue] = BotDecks.GetStarterSlimeDeck(),
            [NPCID.Truffle] = BotDecks.GetMushroomDeck(),
        };
    }
    internal class DuelDialogGlobalNPC : GlobalNPC
    {

        // Hook to check whether the NPC being talked to can be dueled
        public override void GetChat(NPC npc, ref string chat)
        {
            base.GetChat(npc, ref chat);
            if(ModContent.GetInstance<NPCDeckMap>().NPCDecklists.ContainsKey(npc.netID))
            {
                ModContent.GetInstance<UserInterfaces>().StartNPCChat();
            }
        }

    }
}
