using AppCommon.User;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCommon.Game
{
    public class JoinGame
    {
        public required ActorId UserActorId { get; set; }
    }
}
