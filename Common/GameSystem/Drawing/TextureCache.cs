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
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.Drawing
{
    internal class TextureCache : ModSystem
    {

        internal static TextureCache Instance => ModContent.GetInstance<TextureCache>();

        internal Asset<Texture2D> Field { get; private set; }
        internal Asset<Texture2D> Zone { get; private set; }
        internal Asset<Texture2D> ZoneHighlighted { get; private set; }

        internal Asset<Texture2D> ZoneSelectable { get; private set; }

        internal Asset<Texture2D> CardBack { get; private set; }

        internal Asset<Texture2D> OffenseIcon { get; private set; }
        internal Asset<Texture2D> DefenseIcon { get; private set; }
        internal Asset<Texture2D> HeartIcon { get; private set; }

        internal Asset<Texture2D> ManaIcon { get; private set; }
        public Asset<Texture2D> MoveIcon { get; private set; }
        public Asset<Texture2D> Button { get; private set; }
        public Asset<Texture2D> ButtonHighlighted { get; private set; }
        public Asset<Texture2D> StarIcon { get; private set; }
        public Asset<Texture2D> TownsfolkIcon { get; private set; }
        public Asset<Texture2D> PlayerStatsZone { get; private set; }
        public Asset<Texture2D> AttackIcon { get; private set; }
        public Asset<Texture2D> LightRay { get; private set; }
        public Asset<Texture2D> MapBG { get; private set; }
        public Asset<Texture2D> CancelButton { get; private set; }
        public Asset<Texture2D> CardPreviewFrame { get; private set; }
        public Asset<Texture2D> KingSlimeCrown { get; private set; }
        internal Dictionary<int, Asset<Texture2D>> NPCTextureCache { get; private set; }
        internal Dictionary<int, Asset<Texture2D>> ItemTextureCache { get; private set; }

        internal Dictionary<ModifierType, Asset<Texture2D>> ModifierIconTextures { get; private set; }
        public override void Load()
        {
            base.Load();
            Field = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Field");
            Zone = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Zone");
            ZoneHighlighted = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Zone_Highlighted");
            ZoneSelectable = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Zone_Selectable");
            CardBack = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Card_Back");
            OffenseIcon = Main.Assets.Request<Texture2D>("Images/UI/PVP_0");
            DefenseIcon = Main.Assets.Request<Texture2D>("Images/Item_" + ItemID.CobaltShield);
            HeartIcon = Main.Assets.Request<Texture2D>("Images/Item_" + ItemID.Heart);
            ManaIcon = Main.Assets.Request<Texture2D>("Images/Item_" + ItemID.Star);
            MoveIcon = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Boots_Icon");
            Button = Mod.Assets.Request<Texture2D>("Assets/FieldElements/RadialButton");
            ButtonHighlighted = Main.Assets.Request<Texture2D>("Images/UI/Wires_1");
            StarIcon = Main.Assets.Request<Texture2D>("Images/Projectile_" + ProjectileID.FallingStar);
            TownsfolkIcon = Mod.Assets.Request<Texture2D>("Assets/FieldElements/TownsfolkMana");
            PlayerStatsZone = Mod.Assets.Request<Texture2D>("Assets/FieldElements/PlayerStats");
            AttackIcon = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Attack_Icon");
            LightRay = Main.Assets.Request<Texture2D>("Images/Projectile_" + ProjectileID.MedusaHeadRay);
            MapBG = Main.Assets.Request<Texture2D>("Images/MapBG1");
            CancelButton = Mod.Assets.Request<Texture2D>("Assets/FieldElements/CancelGame");
            CardPreviewFrame = Mod.Assets.Request<Texture2D>("Assets/FieldElements/CardPreviewFrame");

            KingSlimeCrown = Main.Assets.Request<Texture2D>("Images/Extra_" + ExtrasID.KingSlimeCrown);
            NPCTextureCache = [];
            ItemTextureCache = [];
            ModifierIconTextures = new Dictionary<ModifierType, Asset<Texture2D>>
            {
                [ModifierType.SPIKED] = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Spiked_Icon"),
                [ModifierType.DEFENSE_BOOST] = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Defense_Icon"),
                [ModifierType.EVASIVE] = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Evasive_Icon"),
                [ModifierType.RELENTLESS] = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Relentless_Icon"),
                [ModifierType.BLEEDING] = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Bleed_Icon"),
                [ModifierType.LIFESTEAL] = Mod.Assets.Request<Texture2D>("Assets/FieldElements/Lifesteal_Icon"),
            };
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
        public Asset<Texture2D> GetItemTexture(int itemId)
        {
            if(!ItemTextureCache.TryGetValue(itemId, out var asset))
            {
                asset = Main.Assets.Request<Texture2D>($"Images/Item_{itemId}");
                ItemTextureCache[itemId] = asset;
            }             
            return asset;
        }
    }
}
