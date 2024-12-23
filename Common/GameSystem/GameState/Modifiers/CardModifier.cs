using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace TerraTCG.Common.GameSystem.GameState.Modifiers
{
    internal enum GameEvent
    {
        // End of the current turn
        END_TURN,

        // After doing an attack
        AFTER_ATTACK,

        // After receiving an attack
        AFTER_RECEIVE_ATTACK,
    }

    internal struct GameEventInfo
    {
        public GameEvent Event { get; set; }
        public bool IsMyTurn { get; set; }
        public GamePlayer TurnPlayer { get; set; }
    }

    internal interface ICardModifier
    {
        public Asset<Texture2D> Texture { get => null; }
        public string Description { get => ""; }

        // Modify an attack as this card performs it
        public void ModifyAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
        {
            // no-op
        }

        // Modify an attack as it is performed against this card
        public void ModifyIncomingAttack(ref Attack attack, Zone sourceZone, Zone destZone) 
        {
            // no-op
        }

        public void ModifyZoneSelection(Zone sourceZone, Zone endZone, ref List<Zone> destZones)
        {
            // no-op
        }

        public void ModifyIncomingZoneSelection(Zone sourceZone, Zone endZone, ref List<Zone> destZones)
        {
            // no-op
        }

        public void ModifyCardEntrance(Zone sourceZone) 
        {
            // no-op
        }

        public bool ShouldRemove(GameEventInfo eventInfo) {
            return false;
        }
    }
}
