using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using AppServiceInterfaces;
using AppCommon.User;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using AppCommon.Game;
using AppCommon.GameModel;

namespace User
{
    internal sealed class User : StatefulService, IUser
    {
        
        public User(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<UserData?> Login(string username, string password)
        {
            var userRepo = await GetUserRepo();

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var existingUser = await userRepo.TryGetValueAsync(tx, username);
                if (!existingUser.HasValue||existingUser.Value.Password!=password)
                {
                    ServiceEventSource.Current.ServiceMessage(this.Context, "User registered: {0}", username);
                    return null;
                }

                return existingUser.Value;
            }
        }

        public async Task<UserData?> Register(string username, string password)
        {
            var userRepo = await GetUserRepo();

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var existingUser = await userRepo.TryGetValueAsync(tx, username);

                if (existingUser.HasValue)
                {
                    ServiceEventSource.Current.ServiceMessage(this.Context, "Can not register user since he already exists: {0}", username);
                    return null;
                }

                var newUser = new UserData() { Username = username, Password = password };
                await userRepo.AddAsync(tx, username, newUser);
                await tx.CommitAsync();

                ServiceEventSource.Current.ServiceMessage(this.Context, "User registered: {0}", username);

                return newUser;
            }
        }

        private async Task<IReliableDictionary<string, UserData>> GetUserRepo()
        {
            return await StateManager.GetOrAddAsync<IReliableDictionary<string, UserData>>("userRepo");
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

    }
}
