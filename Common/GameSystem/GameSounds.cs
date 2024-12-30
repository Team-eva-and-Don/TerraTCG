using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraTCG.Common.GameSystem
{
    internal enum GameAction
    {
        ATTACK,
        PLACE_CARD,
        BOUNCE_CARD,
        PROMOTE_CARD,
        USE_SKILL,
        USE_EQUIPMENT,
        USE_CONSUMABLE,
        TAKE_DAMAGE,
        KILL_ENEMY,
    }
    internal class GameSounds : ModSystem
    {
        internal static GameSounds Instance => ModContent.GetInstance<GameSounds>();

        internal Dictionary<GameAction, SoundStyle> SoundStyles = new()
        {
            [GameAction.PLACE_CARD] = SoundID.DoubleJump,
            [GameAction.USE_SKILL] = SoundID.Item25,
            [GameAction.BOUNCE_CARD] = SoundID.Grab,
            [GameAction.USE_EQUIPMENT] = SoundID.Grab,
            [GameAction.USE_CONSUMABLE] = SoundID.Item3,
            [GameAction.ATTACK] = SoundID.Item1,
            [GameAction.TAKE_DAMAGE] = SoundID.Item10,
            [GameAction.PROMOTE_CARD] = SoundID.Item9,
        };


        internal static void PlaySound(GameAction gameAction)
        {
            if (Instance.SoundStyles.TryGetValue(gameAction, out SoundStyle style))
            {
                SoundEngine.PlaySound(style);
            }
        }
    }
}
