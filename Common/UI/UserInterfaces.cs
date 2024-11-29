using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TerraTCG.Common.UI.GameFieldUI;

namespace TerraTCG.Common.UI
{
    [Autoload(true, Side = ModSide.Client)]
    internal class UserInterfaces : ModSystem
    {
        private UserInterface _userInterface;

        private GameFieldState GameField { get; set; }

        public override void Load()
        {
            GameField = new();
            GameField.Activate();
            _userInterface = new();
            _userInterface.SetState(GameField);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
            _userInterface.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIdx = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if(mouseTextIdx != -1)
            {
                layers.Insert(mouseTextIdx, new LegacyGameInterfaceLayer("TerraTCG: Game Field", delegate
                {
                    if(_userInterface?.CurrentState != null)
                    {
                        _userInterface.Draw(Main.spriteBatch, new GameTime());
                    }
                    return true;
                }, InterfaceScaleType.UI));
            }
        }
    }
}
