using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using AppCommon.Game;
using AppCommon.GameModel;
using System.Net.WebSockets;

namespace Player.Interfaces
{
    public interface IPlayer : IActor
    {
        public Task<GameState?> StartGame(int botNo, CancellationToken cancellationToken);
        public Task<GameState?> JoinGame(ActorId? gameId, CancellationToken cancellationToken);
        public Task<GameState?> GetGame(CancellationToken cancellationToken);
        public Task EndGame(CancellationToken cancellationToken);
        public Task NotifyTurn(GameState gameState, CancellationToken cancellationToken);
        public Task<GameState?> PlayCard(PlayCard playCard, CancellationToken cancellationToken);
    }
}
