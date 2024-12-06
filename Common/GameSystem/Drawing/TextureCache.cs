using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraTCG.Common.GameSystem.Drawing
{
    internal class TextureCache : ModSystem
    {

        internal static TextureCache Instance => ModContent.GetInstance<TextureCache>();

        internal Asset<Texture2D> Field { get; private set; }
        internal Asset<Texture2D> Zone { get; private set; }
        internal Asset<Texture2D> ZoneHighlighted { get; private set; }

        internal Asset<Texture2D> ZoneSelectable { get; private set; }

        internal Asset<Texture2D> OffenseIcon { get; private set; }
        internal Asset<Texture2D> DefenseIcon { get; private set; }
        internal Asset<Texture2D> HeartIcon { get; private set; }

        internal Asset<Texture2D> CostIcon { get; private set; }
        public Asset<Texture2D> Button { get; private set; }
        public Asset<Texture2D> ButtonHighlighted { get; private set; }
        public Asset<Texture2D> StarIcon { get; private set; }
        internal Dictionary<int, Asset<Texture2D>> NPCTextureCache { get; private set; }
        public override void Load()
        {
            base.Load();
            Field = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Field");
            Zone = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Zone");
            ZoneHighlighted = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Zone_Highlighted");
            ZoneSelectable = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Zone_Selectable");
            OffenseIcon = Main.Assets.Request<Texture2D>("Images/UI/PVP_0");
            DefenseIcon = Main.Assets.Request<Texture2D>("Images/Item_" + ItemID.CobaltShield);
            HeartIcon = Main.Assets.Request<Texture2D>("Images/Item_" + ItemID.Heart);
            CostIcon = Main.Assets.Request<Texture2D>("Images/Item_" + ItemID.Star);
            Button = Main.Assets.Request<Texture2D>("Images/UI/Wires_0");
            ButtonHighlighted = Main.Assets.Request<Texture2D>("Images/UI/Wires_1");
            StarIcon = Main.Assets.Request<Texture2D>("Images/Projectile_" + ProjectileID.FallingStar);
            NPCTextureCache = [];
        }

        public Asset<Texture2D> GetNPCTexture(int npcId)
        {
            if(!NPCTextureCache.TryGetValue(npcId, out var asset))
            {
                asset = Main.Assets.Request<Texture2D>($"Images/NPC_{npcId}");
                NPCTextureCache[npcId] = asset;
            }             
            return asset;
        }
    }
}
