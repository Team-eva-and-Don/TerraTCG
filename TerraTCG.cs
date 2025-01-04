using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using TerraTCG.Common.UI;
using TerraTCG.Common.UI.NPCDuelChat;
using TerraTCG.Content.NPCs;
using TerraTCG.CrossMod;

namespace TerraTCG
{
	public class TerraTCG : Mod
	{
        // Set up cross-mod API calls in Dialogue Tweaks for initiating a duel
        // with a duel-able NPC
        public override void PostSetupContent()
        {
            foreach (var duelableNPC in ModContent.GetInstance<NPCDeckMap>().NPCDecklists.Keys)
            {
                DialogueTweakHelper.AddButton(
                    duelableNPC,
                    ()=> Language.GetTextValue("Mods.TerraTCG.Cards.Common.DuelChat"),
                    () => null,
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

    }
}
