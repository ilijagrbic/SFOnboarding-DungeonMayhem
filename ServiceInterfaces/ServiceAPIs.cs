using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceInterfaces
{
    public class ServiceAPIs
    {
        public static readonly Uri _gameActorUri = new Uri("fabric:/DungeonMayhemApp/GameActorService");
        public static readonly Uri _playerActorUri = new Uri("fabric:/DungeonMayhemApp/PlayerActorService");
        public static readonly Uri _userServiceUri = new Uri("fabric:/DungeonMayhemApp/User");
        public static readonly Uri _moveAIServiceUri = new Uri("fabric:/DungeonMayhemApp/MoveAI");
    }
}
