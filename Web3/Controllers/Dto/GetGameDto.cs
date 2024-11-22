using Microsoft.ServiceFabric.Actors;

namespace FrontendService.Controllers.Dto
{
    public class GetGameDto
    {
        public required long GameId { get; set; }
    }
}
