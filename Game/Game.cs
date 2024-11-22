using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using AppServiceInterfaces;
using AppCommon.GameModel;
using AppCommon.Game;
using Player.Interfaces;
using AppCommon;

namespace Game
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class Game : Actor, IGame
    {
        private string GameState = "GameState";
        private IActorTimer notifyTurnTimer;

        public Game(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        public async Task<GameState?> JoinGame(JoinGame joinGame, CancellationToken cancellationToken)
        {
            var fetchResult = await StateManager.TryGetStateAsync<GameState>(GameState, cancellationToken);
            if(!fetchResult.HasValue)
            {
                ActorEventSource.Current.ActorMessage(this, "Trying to join nonexitent game.");
                return null;
            }
            if (fetchResult.Value.AlreadyJoined(joinGame.UserActorId))
            {
                ActorEventSource.Current.ActorMessage(this, "Trying to join same game twice.");
                return null;
            }
            
            fetchResult.Value.AddPlayer(joinGame.UserActorId);
            if (fetchResult.Value.HasEnoughParticipants())
            {
                await ScheduleNotifyTurn(fetchResult.Value.GetPlayerOnTurnId());
            }

            return fetchResult.Value;
        }

        public async Task<GameState?> StartGame(StartGame startGame, CancellationToken cancellationToken)
        {
            GameState newGameState = new GameState(this.Id, startGame);
            await this.StateManager.TryAddStateAsync(GameState, newGameState);

            if (newGameState.HasEnoughParticipants())
            {
                await ScheduleNotifyTurn(newGameState.GetPlayerOnTurnId());
            }

            return newGameState;
        }

        public async Task<GameState?> GetGame(CancellationToken cancellationToken)
        {
            var fetchResult = await StateManager.TryGetStateAsync<GameState>(GameState, cancellationToken);
            if (!fetchResult.HasValue)
            {
                ActorEventSource.Current.ActorMessage(this, "Trying to get nonexitent game.");
                return null;
            }

            return fetchResult.Value;
        }

        public async Task<PlayCardResult?> PlayCard(PlayCard playCard, CancellationToken cancellationToken)
        {
            var fetchResult = await StateManager.TryGetStateAsync<GameState>(GameState, cancellationToken);
            if (!fetchResult.HasValue)
            {
                ActorEventSource.Current.ActorMessage(this, "Trying to play card in a nonexitent game.");
                return null;
            }
            var gameState = fetchResult.Value;
            if (gameState.PlayerStates[fetchResult.Value.CurrentTurn].ActorId != playCard.PlayerId){
                ActorEventSource.Current.ActorMessage(this, "Trying to play card while not on turn.");
                return null;
            }


            var playAnother = gameState.PlayCard(playCard);
            if (gameState.IsGameOver())
            {
                UnregisterTimer(notifyTurnTimer);
                HandleGameOver(gameState);
                return new PlayCardResult { GameState = gameState, playAnother = playAnother };
            }
            if (!playAnother)
            {
                gameState.MoveTurn();
                await ScheduleNotifyTurn(gameState.GetPlayerOnTurnId());
            }

            return new PlayCardResult { GameState = gameState, playAnother = playAnother };
        }

        

        public Task<GameState?> DeleteGame(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            return this.StateManager.TryAddStateAsync("count", 0);
        }
        

        private async Task ScheduleNotifyTurn(ActorId actorId)
        {
            if (notifyTurnTimer != null) { UnregisterTimer(notifyTurnTimer); }
            notifyTurnTimer = RegisterTimer(NotifyPlayerOfTurn, actorId, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(100));
            return;
        }

        private async Task NotifyPlayerOfTurn(Object actorId)
        {
            var fetchResult = await StateManager.TryGetStateAsync<GameState>(GameState, ServiceUtils.GetCancellationToken());
            if (!fetchResult.HasValue)
            {
                ActorEventSource.Current.ActorMessage(this, "Can not notify player turn since GameState is missing!");
                return;
            }
            var gameState = fetchResult.Value;
            ActorEventSource.Current.ActorMessage(this, "Notify player on turn!" + gameState.GetPlayerOnTurnId());
            var gameProxy = ActorProxy.Create<IPlayer>(actorId as ActorId, ServiceAPIs._playerActorUri);
            await gameProxy.NotifyTurn(gameState, ServiceUtils.GetCancellationToken());
            return;
        }

        private void HandleGameOver(GameState gameState)
        {
            ActorEventSource.Current.ActorMessage(this, "Game over!");
            foreach (var player in gameState.PlayerStates)
            {
                var playerProxy = ActorProxy.Create<IPlayer>(player.ActorId, ServiceAPIs._playerActorUri);
                var playerServiceProxy = ActorServiceProxy.Create(ServiceAPIs._playerActorUri, player.ActorId);
                if (player.IsBot)
                {
                    playerServiceProxy.DeleteActorAsync(player.ActorId, ServiceUtils.GetCancellationToken());
                }
                else
                {
                    playerProxy.EndGame(ServiceUtils.GetCancellationToken());
                }
            }
        }
    }
}
