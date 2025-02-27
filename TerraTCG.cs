using System.IO;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using TerraTCG.Common.Netcode;
using TerraTCG.Common.UI;
using TerraTCG.Common.UI.NPCDuelChat;
using TerraTCG.Content.Items;
using TerraTCG.Content.NPCs;
using TerraTCG.CrossMod;

namespace TerraTCG
{
	public class TerraTCG : Mod
	{
        public TerraTCG()
        {
			// Manually load our gores
			GoreAutoloadingEnabled = false;
        }

		public override void Load()
		{
			NetHandler.Load();
		}

		public override void Unload()
		{
			NetHandler.Unload();
		}

		// Set up cross-mod API calls in Dialogue Tweaks for initiating a duel
		// with a duel-able NPC
		public override void PostSetupContent()
        {
            foreach (var duelableNPC in ModContent.GetInstance<NPCDeckMap>().NPCDecklists.Keys)
            {
                DialogueTweakHelper.AddButton(
                    duelableNPC,
                    ()=> Language.GetTextValue("Mods.TerraTCG.Cards.Common.DuelChat"),
                    () => "TerraTCG/Assets/FieldElements/TerraTCGBoosterPack_Small",
                    () =>
                    {
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            ModContent.GetInstance<UserInterfaces>().AdvanceChat();
                        }
                    }
                );
            }
        }

		// Handle a multiplayer data packet
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			NetHandler.HandlePackets(reader, whoAmI);
		}

	}
}
