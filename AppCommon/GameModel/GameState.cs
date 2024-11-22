using AppCommon.Game;
using AppCommon.GameModel.GameLogic;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AppCommon.GameModel
{
    [DataContract]
    public class GameState
    {

        [DataMember]
        [JsonConverter(typeof(ActorIdSerializer))]
        public ActorId GameId { get; set; }
        [DataMember]
        public List<PlayerState> PlayerStates { get; set; }
        [DataMember]
        public int CurrentTurn { get; set; }

        [DataMember]
        public List<HistoryCard> PlayedCards { get; set; }

        public GameState(ActorId actorId, StartGame startGame)
        {
            bool demo = false;
            GameId = actorId;
            this.CurrentTurn = 0;
            PlayerStates = new List<PlayerState>();
            int playerPlacement = demo? 0: new Random().Next(startGame.BotNumber+1);
            for (int i = 0; i < startGame.BotNumber+1; i++)
            {
                if(playerPlacement == i)
                {
                    AddPlayer(startGame.UserActorId);
                }
                else
                {
                    AddBot();
                }
            }
            PlayedCards = new List<HistoryCard>();
        }

        private List<PlayerState.CharacterTypes> GetUsedCharacters()
        {
            var retVal = new List<PlayerState.CharacterTypes>();
            foreach(var playerState in PlayerStates)
            {
                retVal.Add(playerState.CharacterType);
            }

            return retVal;
        }

        private void AddBot()
        {
            PlayerStates.Add(new PlayerState(ActorId.CreateRandom(), true, GetUsedCharacters()));
        }
        public void AddPlayer(ActorId userActorId)
        {
            PlayerStates.Add(new PlayerState(userActorId, false, GetUsedCharacters()));
        }
        public bool HasEnoughParticipants()
        {
            return PlayerStates.Count == 4;
        }
        public bool PlayCard(PlayCard playCard)
        {
            PlayerState? playerState = Find(playCard.PlayerId);
            if(playerState == null)
            {
                throw new ArgumentException("Trying to play card from a player not in the game!" + playCard.PlayerId);
            }

            var deck = playerState.Deck;
            Card cardToPlay = deck.GetCard(playCard.CardIndex);
            var result = new CardResolver { TargetLeft = playCard.TargetLeft }.PlayCard(this, playerState, cardToPlay, playCard.CardIndex);
            RecordHistory(cardToPlay, result.TargetPlayer);

            return result.PlayAnother;
        }

        private void RecordHistory(Card card, int targetPlayer)
        {
            PlayedCards.Insert(0, new HistoryCard { Card = card, TargetPlayer = targetPlayer });
        }

        public PlayerState? Find(ActorId? actorId)
        {
            if(actorId == null)
            {
                return null;
            }
            return PlayerStates.Find(state => state.ActorId == actorId);
        }

        public int FindIndex(ActorId? actorId)
        {
            if (actorId == null)
            {
                return 0;
            }
            for (int i = 0; i < PlayerStates.Count; i++)
            {
                if(PlayerStates[i].ActorId == actorId)
                {
                    return i;
                }
            }
            return 0;
        }

        public PlayerState? FindAliveNeighbour(PlayerState actorPlayer, bool targetLeft)
        {
            int actorIndex = 0;
            for(int i =0; i < PlayerStates.Count; i++)
            {
                if (PlayerStates[i].ActorId == actorPlayer.ActorId)
                {
                    actorIndex = i;
                    break;
                }
            }
            
            bool targetFound = false;
            int index = actorIndex;

            while (!targetFound)
            {
                if (targetLeft) 
                {
                    index--;
                    if(index < 0)
                    {
                        index = 3;
                    }
                }
                else
                {
                    index++;
                    index %= 4;
                }
                if (!PlayerStates[index].IsDead())
                {
                    return PlayerStates[index];
                }
            }
            return null;
        }

        public void MoveTurn()
        {
            CurrentTurn += 1;
            CurrentTurn %= 4;
            if (!PlayerStates[CurrentTurn].IsDead())
            {
                PlayerStates[CurrentTurn].Deck.DrawCards(1);
            }
        }

        public ActorId GetPlayerOnTurnId()
        {
            return PlayerStates[CurrentTurn].ActorId;
        }

        public bool IsGameOver()
        {
            return PlayerStates.FindAll(state => state.HealthPoints <= 0).Count() > 2;
        }

        public bool AlreadyJoined(ActorId userActorId)
        {
            foreach(PlayerState state in PlayerStates)
            {
                if(state.ActorId == userActorId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
