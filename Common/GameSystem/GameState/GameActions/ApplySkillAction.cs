using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.Drawing.Animations.FieldAnimations;
using TerraTCG.Common.Netcode.Packets;
using static TerraTCG.Common.GameSystem.GameState.GameActions.IGameAction;

namespace TerraTCG.Common.GameSystem.GameState.GameActions
{
    internal class ApplySkillAction() : IGameAction
    {
        private Zone zone;

		private Card card;
		private GamePlayer player;

		public ApplySkillAction(Card card, GamePlayer player) : this()
		{
			this.card = card;
			this.player = player;
		}

        public bool CanAcceptZone(Zone zone) => player.Owns(zone) && !zone.IsEmpty() && player.Resources.Mana >= zone.PlacedCard.ModifyIncomingSkill(card).Cost;

        public bool AcceptZone(Zone zone)
        {
            this.zone = zone;
            return true;
        }

        public ActionLogInfo GetLogMessage() => new(card, $"{ActionText("Used")} {card.CardName} {ActionText("On")} {zone.CardName}");

        public string GetZoneTooltip(Zone zone)
        {
            return $"{ActionText("Use")} {card.CardName} {ActionText("On")} {zone.CardName}";
        }

        public string GetCantAcceptZoneTooltip(Zone zone) => player.Owns(zone) && !zone.IsEmpty() ? 
            $"{ActionText("NotEnoughMana")} {ActionText("To")} {ActionText("Use")}" : "";

        public void Complete()
        {
            var showAnimation = new ShowCardAnimation(TCGPlayer.TotalGameTime, card, zone, player == TCGPlayer.LocalGamePlayer);
            var skill = zone.PlacedCard.ModifyIncomingSkill(card);
            player.Game.FieldAnimation = showAnimation;
            if(card.CardType == CardType.ITEM)
            {
                player.Game.CurrentTurn.UsedItemCount += 1;
            }
            var duration = showAnimation.Duration;

            zone.QueueAnimation(new IdleAnimation(zone.PlacedCard, duration: duration));
            zone.QueueAnimation(new ApplyModifierAnimation(zone.PlacedCard, card.Skills[0].Texture));

            card.Skills[0].DoSkill(player, null, zone);
            player.Resources = player.Resources.UseResource(mana: skill.Cost);
            player.Hand.Remove(card);
            if(card.SubTypes.Contains(CardSubtype.EQUIPMENT))
            {
                GameSounds.PlaySound(GameAction.USE_EQUIPMENT);
            } else
            {
                GameSounds.PlaySound(GameAction.USE_CONSUMABLE);
            }
        }
		public void Send(BinaryWriter writer)
		{
			writer.Write(player.Index);
			writer.Write(CardNetworkSync.Serialize(card));
		}

		public void Receive(BinaryReader reader, CardGame game)
		{
			player = game.GamePlayers[reader.ReadByte()];
			card = CardNetworkSync.Deserialize(reader.ReadUInt16());
		}
    }
}
