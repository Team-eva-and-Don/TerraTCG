using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI;
using TerraTCG.Common.GameSystem.Drawing;
using TerraTCG.Common.UI.Common;

namespace TerraTCG.Common.UI.NPCDuelChat
{
    internal class NPCDuelChatButton : CustomClickUIElement
    {
        public float TextScale => ContainsPoint(Main.MouseScreen) ? 1f : 0.9f;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var font = FontAssets.MouseText;
            var chatColor = new Color(244, 222, 122) * (Main.mouseTextColor / 255f);
            var buttonText = Language.GetTextValue("Mods.TerraTCG.Common.DuelChat");
            var bgColor = ContainsPoint(Main.MouseScreen) ? new Color(165, 42, 42) : Color.Black;
            var center = Position + new Vector2(Width.Pixels, Height.Pixels) / 2;
            CardTextRenderer.Instance.DrawStringWithBorder(spriteBatch, buttonText, center, centered: true, color: chatColor, bgColor: bgColor, font: font.Value, scale: TextScale);
        }
    }
}
