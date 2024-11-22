using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCommon.User
{
    [DataContract]
    public class UserData
    {
        [DataMember]
        public required string Username { get; set; }

        [DataMember]
        public required string Password { get; set; }
    }
}
