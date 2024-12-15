using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState.Modifiers
{
    internal class NoOpModifier : ICardModifier
    {
        public string Texture => "";

        public string Description => "";
    }
}
