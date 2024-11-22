using AppCommon.Game;
using AppCommon.GameModel;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCommon.GameModel.GameLogic;

namespace AppServiceInterfaces
{
    public interface IMoveAI : IService
    {
        Task<PlayCard> DetermineMove(GameState gameState, AIDificulty aIDificuly, CancellationToken cancellationToken);
    }
}
