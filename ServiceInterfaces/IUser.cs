using AppCommon.GameModel;
using AppCommon.User;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppServiceInterfaces
{
    public interface IUser: IService
    {
        public Task<UserData?> Register(string username, string password);

        public Task<UserData?> Login(string username, string password);
    }
}
