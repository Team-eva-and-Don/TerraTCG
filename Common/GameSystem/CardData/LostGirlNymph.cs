using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using TerraTCG.Common.GameSystem.Drawing.Animations;
using TerraTCG.Common.GameSystem.GameState;
using TerraTCG.Common.GameSystem.GameState.Modifiers;

namespace TerraTCG.Common.GameSystem.CardData
{
    internal class LostGirl : ModSystem, ICardTemplate
    {
        private class TransformLostGirlModifier : ICardModifier
        {
            // TODO prefer not to queue animations in a modifier's logic
            public bool ShouldRemove(GameEventInfo eventInfo) {
                // check if the most recently drawn card is an item
                bool didDrawItem = eventInfo.TurnPlayer.Hand.Cards.LastOrDefault()?.CardType == CardType.ITEM;
                if(eventInfo.IsMyTurn && eventInfo.Event == GameEvent.START_TURN && didDrawItem)
                {
                    var leavingCard = eventInfo.Zone.PlacedCard;
                    var dmgTaken = leavingCard.Template.MaxHealth - leavingCard.CurrentHealth;
                    var nymphCard = ModContent.GetInstance<Nymph>().CreateCard();

                    eventInfo.Zone.PlaceCard(nymphCard);
                    eventInfo.Zone.PlacedCard.IsExerted = false;
                    eventInfo.Zone.PlacedCard.CurrentHealth -= dmgTaken;

                    eventInfo.Zone.QueueAnimation(new RemoveCardAnimation(leavingCard));
                    eventInfo.Zone.QueueAnimation(new PlaceCardAnimation(eventInfo.Zone.PlacedCard));
                }
                return false;
            }
        }

        public Card CreateCard() => new ()
        {
            Name = "LostGirl",
            MaxHealth = 6,
            MoveCost = 2,
            NPCID = NPCID.LostGirl,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.CASTER],
            Modifiers = [
                new TransformLostGirlModifier(),
            ],
            Attacks = [
                new() {
                    Name = "Lure",
                    Damage = 1,
                    Cost = 1,
                }
            ]
        };
    }

    internal class Nymph : ModSystem, ICardTemplate
    {
        public Card CreateCard() => new ()
        {
            Name = "Nymph",
            MaxHealth = 8,
            MoveCost = 2,
            NPCID = NPCID.Nymph,
            CardType = CardType.CREATURE,
            SubTypes = [CardSubtype.CAVERN, CardSubtype.CASTER],
            Modifiers = [
                new EvasiveModifier(),
            ],
            Attacks = [
                new() {
                    Name = "Ensnare",
                    Damage = 5,
                    Cost = 2,
                }
            ]
        };
    }
}
