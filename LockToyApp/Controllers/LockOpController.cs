
using LockToyApp.DTOs;
using LockToyApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ToyContracts;

namespace LockToyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LockOpController : ControllerBase
    {
        public Settings Settings { get; }
        public readonly IUserService userService;
        public readonly IDoorHistoryService doorHistoryService;
        public readonly IDoorOperationSender doorOperationSender;
        public LockOpController(IOptionsSnapshot<Settings> settings, IUserService userService, IDoorHistoryService doorHistoryService, IDoorOperationSender doorOperationSender)
        {
            this.Settings = settings.Value;
            this.userService = userService;
            this.doorHistoryService = doorHistoryService;
            this.doorOperationSender = doorOperationSender;
        }

        [HttpGet("GetUserByName")]
        public async Task<ActionResult<UserDto>?> Get([FromHeader(Name ="username")] string userName, [FromHeader(Name ="password")] string password)
        {
            var userIsValide = await this.userService.IsUserValid(userName, password);
            if (!userIsValide)
            {
                return this.Unauthorized();
            }

            var user = await this.userService.GetUserByName(userName);
            if (user != null)
            {
                var registrations = user.UserRegistrations;
                var doorIds = registrations.Select(r => r.DoorID).ToList();

                return new UserDto() { UserName = user.UserName, UserType = user.UserType, DoorIds = doorIds };
            }
            return null;
        }

        [HttpPost("OpenDoor")]
        public async Task<ActionResult<string>?> Post([FromHeader(Name = "username")] string userName, [FromHeader(Name = "password")] string password, string doorId)
        {
            Guid parsedDoorId;
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(doorId) || !Guid.TryParse(doorId, out parsedDoorId))
            {
                return this.BadRequest();
            }
            var userIsValide = await this.userService.IsUserValid(userName, password);
            if (!userIsValide)
            {
                return this.Unauthorized();
            }

            
            var user = await this.userService.GetUserByName(userName);
            if (user != null)
            {
                var registrations = user.UserRegistrations;
                var doorIds = registrations.Select(r => r.DoorID).ToList();

                if (doorIds.Contains(parsedDoorId))
                {
                    var doorOpenRequest = new ToyContracts.DoorOpRequest() 
                    {
                        UserId = user.UserID,
                        DoorId = parsedDoorId,
                        UserName = user.UserName
                    };
                    var isSuccessful = await this.doorOperationSender.SendOperationAsync(doorOpenRequest);
                    var result = isSuccessful ? "successful" : "failed";
                    return this.Ok($"sent door open request with result {result}");
                }

            }
            return null;
        }

        [HttpGet("DoorHistory")]
        public async Task<ActionResult<List<HistoryDto>>> GetDoorHistory([FromHeader(Name = "username")] string userName, [FromHeader(Name = "password")] string password, string doorId)
        {
            Guid parsedDoorId;
            var historyToReturn = new List<HistoryDto>();
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(doorId) || !Guid.TryParse(doorId, out parsedDoorId))
            {
                return this.BadRequest();
            }

            var userIsValide = await this.userService.IsUserValid(userName, password);
            var foundUser= await this.userService.GetUserByName(userName);

            if (!userIsValide || foundUser == null || foundUser.UserType != Models.UserType.Administrator)
            {
                return this.Unauthorized();
            }

            var doorHistory = await this.doorHistoryService.GetDoorHistoryItemsAsync(doorId);

            if (doorHistory.Any())
            {
                foreach (var item in doorHistory)
                {
                    historyToReturn.Add(new HistoryDto 
                    {
                        DoorAction = item.Operation,
                        UserName = item.UserName,
                        OperationTime = item.OperationTime
                    });
                }
            }

            return historyToReturn;

        }


    }
}
