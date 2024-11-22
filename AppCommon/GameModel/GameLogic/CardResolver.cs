using AppCommon.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AppCommon.GameModel.CardEffect;

namespace AppCommon.GameModel.GameLogic
{
    public class CardResolver
    {
        public class PlayCardResult
        {
            public bool PlayAnother { get; set; }
            public int TargetPlayer { get; set; }
        }
        public bool TargetLeft {set;get;}
        public PlayCardResult PlayCard(GameState gameState, PlayerState playerState, Card card, int cardIndex)
        {
            bool anotherCard  = false;
            int targetPlayer = -1;
            foreach (CardEffect cardEffect in card.CardEffects)
            {
                int value = cardEffect.Value;
                switch (cardEffect.CardEffectType)
                {
                    case CardEffectTypes.Draw:
                        handleDraw(playerState, value);
                        break;
                    case CardEffectTypes.Damage:
                        targetPlayer = handleDamage(gameState, playerState, value);
                        break;
                    case CardEffectTypes.ExtraCard:
                        anotherCard = true;
                        break;
                    case CardEffectTypes.Heal:
                        handleHeal(playerState, value);
                        break;
                    case CardEffectTypes.Block:
                        handleBlock(playerState, cardIndex, value);
                        break;
                    default:
                        throw new ArgumentException("Unsupported effect!");
                }
            }
            playerState.Deck.PlayCard(cardIndex);

            if (playerState.Deck.Hand.Count == 0)
            {
                playerState.Deck.DrawCards(playerState.IsDead()?1:2);
            }

            return new PlayCardResult { PlayAnother = anotherCard, TargetPlayer = targetPlayer };
        }

        private void handleBlock(PlayerState playerState, int cardIndex, int value)
        {
            playerState.AddBlock(value);
        }
        private void handleHeal(PlayerState playerState, int value)
        {
            playerState.Heal(value);
        }
        private void handleDraw(PlayerState playerState, int value)
        {
            playerState.Deck.DrawCards(value);
        }
        private int handleDamage(GameState gameState, PlayerState playerState,int value)
        {
            PlayerState? neighbour = gameState.FindAliveNeighbour(playerState, TargetLeft);
            if(neighbour == null)
            {
                throw new ArgumentException("Trying to target neighbour but none found, game must be over!");
            }
            neighbour.Damage(value);

            return gameState.FindIndex(neighbour?.ActorId);
        }
    }
}
