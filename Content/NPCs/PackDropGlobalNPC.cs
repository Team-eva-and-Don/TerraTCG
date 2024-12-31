using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Content.Items;

namespace TerraTCG.Content.NPCs
{
    internal class PackDropGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // Give a small chance for any NPC to drop a TerraTCGPack
            if (!NPCID.Sets.CountsAsCritter[npc.type])
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TerraTCGBoosterPack>(), 40));
            }
        }
    }
}
