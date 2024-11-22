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

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace AppServiceInterfaces
{
    public interface IGame : IActor
    {
        Task<GameState?> StartGame(StartGame startGame, CancellationToken cancellationToken);

        Task<GameState?> JoinGame(JoinGame joinGame, CancellationToken cancellationToken);

        Task<GameState?> GetGame(CancellationToken cancellationToken);
        
        Task<PlayCardResult?> PlayCard(PlayCard playCard, CancellationToken cancellationToken);
        
        Task<GameState?> DeleteGame(CancellationToken cancellationToken);
    }
}
