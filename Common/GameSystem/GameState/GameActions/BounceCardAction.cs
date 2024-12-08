using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing.Animations;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    internal class BounceCardAction(Card card, GamePlayer player) : TownsfolkAction(card, player)
    {
        private Zone zone;

        public override bool CanAcceptZone(Zone zone) => base.CanAcceptZone(zone) && Player.Owns(zone) && !zone.IsEmpty();

        public override bool AcceptZone(Zone zone)
        {
            this.zone = zone;
            return true;
        }

        public override void Complete()
        {
            base.Complete();
            zone.Animation = new RemoveCardAnimation(zone, zone.PlacedCard, Main._drawInterfaceGameTime.TotalGameTime);
            Player.Hand.Add(zone.PlacedCard.Template);
            zone.PlacedCard = null;
        }
    }
}
