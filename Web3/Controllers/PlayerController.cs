using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FrontendService.Controllers.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Collections.Concurrent;
using AppServiceInterfaces;
using Player.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors;
using AppCommon;
using AppCommon.GameModel;
using AppCommon.Game;
using Frontend;

namespace FrontendService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        [HttpPost]
        [Route("startGame")]
        public async Task<IActionResult> StartGame([FromBody] StartGameDto body, CancellationToken cancellationToken)
        {
            var playerProxy = ActorProxy.Create<IPlayer>(new ActorId(body.Username), ServiceAPIs._playerActorUri);
            var response = await playerProxy.StartGame(body.BotNo, ServiceUtils.GetCancellationToken());
            
            if (response == null)
            {
                return BadRequest(response);
            }
            
            return Ok(response);
        }

        [HttpPost]
        [Route("joinGame")]
        public async Task<IActionResult> JoinGame([FromBody] JoinGameDto body, CancellationToken cancellationToken)
        {
            var playerProxy = ActorProxy.Create<IPlayer>(new ActorId(body.Username), ServiceAPIs._playerActorUri);
            var response = await playerProxy.JoinGame(ActorIdSerializer.ConvertToActorId(body.GameId), ServiceUtils.GetCancellationToken());

            if (response == null)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut]
        [Route("getGame")]
        public async Task<GameState?> GetGame([FromBody] GetGameDto body, CancellationToken cancellationToken)
        {
            var proxy = ActorProxy.Create<IGame>(new ActorId(body.GameId), ServiceAPIs._gameActorUri);
            return await proxy.GetGame(ServiceUtils.GetCancellationToken());
        }
        [HttpPut]
        [Route("playCard")]
        public async Task<GameState?> PlayCard([FromBody] PlayCardDto body, CancellationToken cancellationToken)
        {
            var proxy = ActorProxy.Create<IPlayer>(ActorIdSerializer.ConvertToActorId(body.PlayerId), ServiceAPIs._playerActorUri);
            return await proxy.PlayCard(new PlayCard { PlayerId = ActorIdSerializer.ConvertToActorId(body.PlayerId), CardIndex = body.CardIndex, TargetLeft = body.TargetLeft }, ServiceUtils.GetCancellationToken());
        }
    }
}
