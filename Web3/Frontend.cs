using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Data;
using System.Text.Json.Serialization;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors;
using System.Net.WebSockets;
using Player.Interfaces;
using AppServiceInterfaces;
using System.Text;
using System.Runtime.Serialization;
using AppCommon.GameModel;
using System.Xml;
using System.Runtime.Serialization.Json;

namespace Frontend
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class Frontend : StatelessService
    {
        public Frontend(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var builder = WebApplication.CreateBuilder();

                        builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);
                        builder.WebHost
                                    .UseKestrel()
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url);
                        builder.Services.AddControllers().AddJsonOptions(x =>
                        {
                            x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        });
                        builder.Services.AddControllersWithViews();
                        var app = builder.Build();
                        if (!app.Environment.IsDevelopment())
                        {
                        }
                        app.UseStaticFiles();
                        app.UseRouting();
                        app.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action=Index}/{id?}");
                        app.MapFallbackToFile("index.html");
                        app.UseWebSockets();
                         app.Use(async (context, next) =>
                            {
                                if (context.Request.Path.StartsWithSegments("/ws", out var remainingPath))
                                {
                                    if (context.WebSockets.IsWebSocketRequest)
                                    {
                                        var pathVariable = remainingPath.Value.Trim('/');
                                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                                        var actorId = new ActorId(pathVariable);
                                        var actor = ActorProxy.Create<IPlayer>(actorId, ServiceAPIs._playerActorUri);
                                        while (true)
                                        {
                                            if (webSocket != null && webSocket.State == WebSocketState.Open)
                                            {
                                                var gameState = await actor.GetGame(CancellationToken.None);
                                                if(gameState != null){
                                                    string jsonString = SerializeToJsonString<GameState>(gameState);
                                                    var buffer = Encoding.UTF8.GetBytes(jsonString);
                                                    await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                                                }
                                            }
                                            Thread.Sleep(2000);
                                        }
                                    }
                                    else
                                    {
                                        context.Response.StatusCode = 400;
                                    }
                                }
                                else
                                {
                                    await next(); 
                                }
                            });

                        return app;

                    }))
            };
        }

        public static string SerializeToJsonString<T>(T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(memoryStream, obj);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

    }
}
