using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    internal delegate IGameAction SelectInHandAction(Card card, GamePlayer gamePlayer);

    internal delegate IGameAction SelectOnFieldAction(Zone zone, GamePlayer gamePlayer);

    internal readonly struct ActionLogInfo(Card card, string message)
    {
        public Card Card { get; } = card;
        public string Message { get; } = message;
    }

    // Interface for a state machine that accepts player input until
    // enough info has been gathered to perform an action
    internal interface IGameAction
    {
        public bool CanAcceptZone(Zone zone) => false;
        public bool AcceptZone(Zone zone) => false;

        public bool CanAcceptCardInHand(Card card) => false;

        public bool AcceptCardInHand(Card card) => false;

        public bool CanAcceptActionButton() => false;

        public bool AcceptActionButton() => false;

        public void Complete();

        public ActionLogInfo GetLogMessage();

        public Color HighlightColor(Zone zone)
        {
            return TCGPlayer.LocalGamePlayer.Owns(zone) ? Color.LightSkyBlue : Color.LightCoral;
        }

        public void Cancel() 
        {
            // No-op
        }

        public static string ActionText(string key)
            => Language.GetTextValue($"Mods.TerraTCG.Cards.ActionText.{key}");
    }
}
