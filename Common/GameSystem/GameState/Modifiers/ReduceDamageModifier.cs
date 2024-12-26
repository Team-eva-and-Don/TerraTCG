using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.GameSystem.GameState.Modifiers
{
    internal class ReduceDamageModifier(int amount, List<GameEvent> removeOn = null) : ICardModifier
    {
        public int Amount => amount;

        public Asset<Texture2D> Texture { get; set; }
        public CardSubtype Source { get; set; }
        public ModifierType Category => ModifierType.DEFENSE_BOOST;

        public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
        {
            attack.Damage = Math.Max(1, attack.Damage - amount);
        }

        public bool ShouldRemove(GameEventInfo eventInfo) =>
            !eventInfo.IsMyTurn && (removeOn?.Contains(eventInfo.Event) ?? false);

    }
}
