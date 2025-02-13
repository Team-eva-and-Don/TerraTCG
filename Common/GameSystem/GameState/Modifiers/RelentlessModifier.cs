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
        public int Amount => 1;

        public Asset<Texture2D> Texture { get; set; }
		public Card SourceCard { get; set; }
        public ModifierType Category => ModifierType.RELENTLESS;

        public string Description => "";

		private readonly bool[] doUnpause = [true, true, true, true, true, true];

        private Zone sourceZone;

        public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
        {
            this.sourceZone = sourceZone;
        }

        public bool ShouldRemove(GameEventInfo eventInfo) {
            if(eventInfo.Event == GameEvent.AFTER_ATTACK && doUnpause[eventInfo.Zone.Index] && sourceZone?.PlacedCard is PlacedCard card && card.IsExerted)
            {
                card.IsExerted = false;
                sourceZone.QueueAnimation(new BecomeActiveAnimation(card));
				doUnpause[eventInfo.Zone.Index] = false;
            }
            if(eventInfo.Event == GameEvent.END_TURN)
            {
				Array.Fill(doUnpause, true);
            }
            return false;
        }
    }
}
