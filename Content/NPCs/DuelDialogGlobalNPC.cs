using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerraTCG.Common.UI;

namespace TerraTCG.Content.NPCs
{
    internal class DuelDialogGlobalNPC : GlobalNPC
    {

        // Hook to check whether the NPC being talked to can be dueled
        public override void GetChat(NPC npc, ref string chat)
        {
            base.GetChat(npc, ref chat);
            ModContent.GetInstance<UserInterfaces>().StartNPCChat();
        }

    }
}
