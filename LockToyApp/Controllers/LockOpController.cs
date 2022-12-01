

using LockToyApp.Attributes;
using LockToyApp.DTOs;
using LockToyApp.Models;
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

        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate(AuthenticationRequest request)
        {
            var response = await this.userService.Authenticate(request);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [CrazybeeAuthorize]
        [HttpGet("GetUserByName")]
        public async Task<ActionResult<UserDto>?> Get(string userName)
        {
            
            var user = await this.userService.GetUserByName(userName);
            var userInContext = this.HttpContext.Items["User"] as DBEntities.User;
            if (user == null || userInContext == null || user != userInContext)
            {
                return null;
            }

            var registrations = user.UserRegistrations;
            var doorIds = registrations.Select(r => r.DoorID).ToList();

            return new UserDto() { UserName = user.UserName, UserType = user.UserType, DoorIds = doorIds };
            
        }

        [CrazybeeAuthorize]
        [HttpPost("OpenDoor")]
        public async Task<ActionResult?> Post([FromBody] DoorRequest doorRequest)
        {
            Guid parsedDoorId;
            if (!Guid.TryParse(doorRequest?.DoorId, out parsedDoorId))
            {
                return this.BadRequest();
            }
            var user = await this.userService.GetUserByName(doorRequest.UserName);
            var userInContext = this.HttpContext.Items["User"] as DBEntities.User;

            if (userInContext != user || user == null)
            {
                return null;
            }

          
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

            return null;

        }

        [CrazybeeAuthorize]
        [HttpGet("DoorHistory")]
        public async Task<ActionResult<List<HistoryDto>>?> GetDoorHistory([FromBody] DoorRequest doorRequest)
        {
           
            var historyToReturn = new List<HistoryDto>();
            var foundUser = await this.userService.GetUserByName(doorRequest.UserName);
            if (foundUser == null)
            {
                return null;
            }

            var userInContext = this.HttpContext.Items["User"] as DBEntities.User;

            if (userInContext == null || foundUser.UserType != UserType.Administrator || userInContext != foundUser)
            {
                return this.Unauthorized();
            }

            var doorHistory = await this.doorHistoryService.GetDoorHistoryItemsAsync(doorRequest.DoorId);

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
