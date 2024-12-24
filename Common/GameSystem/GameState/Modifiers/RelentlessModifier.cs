using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraTCG.Common.GameSystem.Drawing.Animations;

namespace TerraTCG.Common.GameSystem.GameState.Modifiers
{
    internal class RelentlessModifier : ICardModifier
    {
        public Asset<Texture2D> Texture { get; set; }

        public string Description => "";

        // TODO storing state in a modifier is not best practice
        private bool doUnpause = true;
        private Zone sourceZone;

        public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
        {
            this.sourceZone = sourceZone;
        }

        public bool ShouldRemove(GameEventInfo eventInfo) {
            if(eventInfo.Event == GameEvent.AFTER_ATTACK && doUnpause && sourceZone?.PlacedCard is PlacedCard card)
            {
                card.IsExerted = false;
                sourceZone.QueueAnimation(new BecomeActiveAnimation(card));
                doUnpause = false;
            }
            if(eventInfo.Event == GameEvent.END_TURN)
            {
                doUnpause = true;
            }
            return false;
        }
    }
}
