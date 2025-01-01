using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace TerraTCG.Common.Configs
{
    internal class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("GameUI")]
        [DefaultValue(true)]
        public bool ShowActionLog;

        [DefaultValue(true)]
        public bool ShowTooltips;

    }
}
