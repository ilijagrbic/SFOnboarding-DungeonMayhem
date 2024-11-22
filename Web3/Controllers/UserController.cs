using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FrontendService.Controllers.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Collections.Concurrent;
using AppServiceInterfaces;
using System.Security.Cryptography;
using System.Text;

namespace FrontendService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserInfoDto body, CancellationToken cancellationToken)
        {
            var userService = ServiceProxy.Create<IUser>(ServiceAPIs._userServiceUri, new ServicePartitionKey(GeneratePartitionKey(body)));
            var response = await userService.Register(body.Username, body.Password);

            if (response == null)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserInfoDto body, CancellationToken cancellationToken)
        {
            var userService = ServiceProxy.Create<IUser>(ServiceAPIs._userServiceUri, new ServicePartitionKey(GeneratePartitionKey(body)));
            var response = await userService.Login(body.Username, body.Password);

            if (response==null)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }

        public static long GeneratePartitionKey(UserInfoDto userInfo)
        {
            string combined = userInfo.Username + userInfo.Password;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                long partitionKey = BitConverter.ToInt64(hashBytes, 0);
                return partitionKey;
            }
        }

    }
}
