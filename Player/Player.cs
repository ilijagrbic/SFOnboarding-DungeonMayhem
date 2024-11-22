using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Player.Interfaces;
using AppCommon.Game;
using AppCommon.GameModel;
using AppServiceInterfaces;
using Microsoft.ServiceFabric.Data;
using AppCommon;
using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Client;
using AppCommon.GameModel.GameLogic;

namespace Player
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class Player : Actor, IPlayer
    {

        private string CurrentGameState = "GameId";
        private string GamesHistoryState = "HistoryGameIds";
        private string DifficultyState = "Dificulty";

        private IActorTimer turnTimer;

        public Player(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            if (IsBot())
            {
                Random random = new Random();
                Array values = Enum.GetValues(typeof(AIDificulty));
                // TODO support other dificulties
                AIDificulty randomDificulty = (AIDificulty)values.GetValue(random.Next(1));
                StateManager.TryAddStateAsync(DifficultyState, randomDificulty);
            }
            else
            {
                StateManager.TryAddStateAsync(DifficultyState, AIDificulty.Dummy);
            }

            return Task.CompletedTask;
        }

        protected override Task OnDeactivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor deactivated.");
            if (IsBot())
            {
                StateManager.RemoveStateAsync(GamesHistoryState);
            }
            StateManager.RemoveStateAsync(CurrentGameState);
            return Task.CompletedTask;
        }

        public async Task<GameState?> StartGame(int botNo, CancellationToken cancellationToken)
        {
            if (botNo > 3)
            {
                ActorEventSource.Current.ActorMessage(this, "Can not create game with more than 3 bots!");
                return null;
            }

            var currentGameId = await StateManager.TryGetStateAsync<ActorId>(CurrentGameState, cancellationToken);
            if (currentGameId.HasValue)
            {
                ActorEventSource.Current.ActorMessage(this, "Can not create game while player already in game!");
                return null;
            }

            var newGameId = ActorId.CreateRandom();
            var gameProxy = ActorProxy.Create<IGame>(newGameId, ServiceAPIs._gameActorUri);
            var gameStateResult = await gameProxy.StartGame(new StartGame { BotNumber = botNo, UserActorId = this.GetActorId() }, ServiceUtils.GetCancellationToken());

            await StateManager.TryAddStateAsync(CurrentGameState, newGameId);
            await StateManager.SetStateAsync(CurrentGameState, newGameId);

            return gameStateResult;
        }

        public async Task<GameState?> JoinGame(ActorId? gameId, CancellationToken cancellationToken)
        {
            var currentGameId = await StateManager.TryGetStateAsync<ActorId>(CurrentGameState, cancellationToken);
            if (currentGameId.HasValue)
            {
                ActorEventSource.Current.ActorMessage(this, "Can not create game while player in game!");
                return null;
            }
            if (gameId == null)
            {
                ActorEventSource.Current.ActorMessage(this, "Can not create game gameId to join is null!");
                return null;
            }

            var gameProxy = ActorProxy.Create<IGame>(gameId, ServiceAPIs._gameActorUri);
            var gameStateResult = await gameProxy.JoinGame(new JoinGame { UserActorId = this.GetActorId() }, ServiceUtils.GetCancellationToken());

            await StateManager.TryAddStateAsync(CurrentGameState, gameId);
            await StateManager.SetStateAsync(CurrentGameState, gameId);

            return gameStateResult;
        }
        public async Task<GameState?> GetGame(CancellationToken cancellationToken)
        {
            var currentGameId = await StateManager.TryGetStateAsync<ActorId>(CurrentGameState, cancellationToken);
            if (!currentGameId.HasValue)
            {
                ActorEventSource.Current.ActorMessage(this, "Can not get game that doesnt exist!");
                return null;
            }

            var gameProxy = ActorProxy.Create<IGame>(currentGameId.Value, ServiceAPIs._gameActorUri);
            var gameStateResult = await gameProxy.GetGame(ServiceUtils.GetCancellationToken());

            return gameStateResult;
        }

        public async Task NotifyTurn(GameState gameState, CancellationToken cancellationToken)
        {
            var currentGameId = await StateManager.TryGetStateAsync<ActorId>(CurrentGameState, cancellationToken);
            if (!currentGameId.HasValue || currentGameId.Value != gameState.GameId)
            {
                await StateManager.AddStateAsync(CurrentGameState, gameState.GameId);
            }
            turnTimer = RegisterTimer(ResolveTurn, gameState, TimeSpan.FromSeconds(TurnSeconds()), TimeSpan.FromDays(1));
            return;
        }

        private async Task ResolveTurn(object work)
        {
            var currentGameId = await StateManager.TryGetStateAsync<ActorId>(CurrentGameState);
            if (!currentGameId.HasValue || currentGameId.Value != (work as GameState).GameId)
            {
                ActorEventSource.Current.ActorMessage(this, "Error while resolving the turn!");
                return;
            }
            var gameProxy = ActorProxy.Create<IGame>(currentGameId.Value, ServiceAPIs._gameActorUri);
            var aiProxy = ServiceProxy.Create<IMoveAI> (ServiceAPIs._moveAIServiceUri);
            bool playCard = true;
            while (playCard)
            {
                var move = await aiProxy.DetermineMove(work as GameState, AppCommon.GameModel.GameLogic.AIDificulty.Dummy, ServiceUtils.GetCancellationToken());
                if (move != null)
                {
                    var result = await gameProxy.PlayCard(move, ServiceUtils.GetCancellationToken());
                    playCard = result.playAnother;
                }
            }
            UnregisterTimer(turnTimer);
            return;
        }

        private bool IsBot()
        {
            return this.Id.Kind == ActorIdKind.Long;
        }

        private int TurnSeconds()
        {
            if (IsBot())
            {
                return new Random().Next(2)+4;
            }
            else
            {
                return 60;
            }
        }

        public async Task<GameState?> PlayCard(PlayCard playCard, CancellationToken cancellationToken)
        {
            var currentGameId = await StateManager.TryGetStateAsync<ActorId>(CurrentGameState);
            if (!currentGameId.HasValue)
            {
                ActorEventSource.Current.ActorMessage(this, "Can not play card if the game doesnt exist!");
                return null;
            }
            var gameProxy = ActorProxy.Create<IGame>(currentGameId.Value, ServiceAPIs._gameActorUri);
            var result = await gameProxy.PlayCard(playCard, ServiceUtils.GetCancellationToken());
            UnregisterTimer(turnTimer);
            return result.GameState;
        }

        public async Task EndGame(CancellationToken cancellationToken)
        {
            if (!IsBot())
            {
                var currentGameId = await StateManager.TryGetStateAsync<ActorId>(CurrentGameState);
                if (!currentGameId.HasValue)
                {
                    ActorEventSource.Current.ActorMessage(this, "Can not end game if the game doesnt exist!");
                    return;
                }
                var historyGameIds = await StateManager.TryGetStateAsync<List<ActorId>>(GamesHistoryState);
                if (historyGameIds.HasValue)
                {
                    historyGameIds.Value.Add(currentGameId.Value);
                    await StateManager.SetStateAsync<List<ActorId>>(GamesHistoryState, historyGameIds.Value);
                }
                else
                {
                    var initHistory = new List<ActorId>();
                    initHistory.Add(currentGameId.Value);
                    await StateManager.SetStateAsync<List<ActorId>>(GamesHistoryState, initHistory);
                }
                await StateManager.RemoveStateAsync(CurrentGameState);
                return;
            }
        }

    }
}
