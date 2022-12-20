using LockToyApp.Attributes;
using LockToyApp.DTOs;
using LockToyApp.Models;
using LockToyApp.Services;
using Microsoft.AspNetCore.Mvc;
using ToyContracts;

namespace LockToyApp.Controllers
{
    [Route("api/lockoperation")]
    [ApiController]
    public class LockOpController : BaseController
    {
        public readonly IDoorHistoryService doorHistoryService;
        public readonly IDoorOperationSender doorOperationSender;
        public LockOpController(IUserService userService, IDoorHistoryService doorHistoryService, IDoorOperationSender doorOperationSender) : base(userService)
        {
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
        [HttpGet("user")]
        public async Task<ActionResult<UserDto>?> Get(string userName)
        {
           
            var validUser = await IsUserValidInContext(userName);
            if (validUser == null)
            {
                return null;
            }

            var registrations = await this.userService.GetUserRegistrationsFromCache(userName);
            var doorIds = registrations?.Select(r => r.DoorID).ToList();

            return new UserDto() { UserName = validUser.UserName, UserType = validUser.UserType.ToString(), DoorIds = doorIds };
            
        }

        [CrazybeeAuthorize]
        [HttpPost("opendoor")]
        public async Task<ActionResult?> Post([FromBody] DoorRequest doorRequest)
        {
            Guid parsedDoorId;
            var validUser = await IsUserValidInContext(doorRequest.UserName);
            if (!Guid.TryParse(doorRequest?.DoorId, out parsedDoorId) || validUser == null)
            {
                return this.BadRequest();
            }

            var registrations = await this.userService.GetUserRegistrationsFromCache(doorRequest.UserName);
            var doorIds = registrations?.Select(r => r.DoorID).ToList();

            if (doorIds != null && doorIds.Contains(parsedDoorId))
            {
                var doorOpenRequest = new DoorOpRequest()
                {
                    UserId = validUser.UserID,
                    DoorId = parsedDoorId,
                    UserName = validUser.UserName
                };
                var isSuccessful = await this.doorOperationSender.SendOperationAsync(doorOpenRequest);
                var result = isSuccessful ? "successful" : "failed";
                return this.Ok($"sent door open request with result {result}");
            }

            return this.NotFound();

        }

        [CrazybeeAuthorize]
        [HttpGet("doorhistory")]
        public async Task<ActionResult<List<HistoryDto>>?> GetDoorHistory([FromBody] DoorRequest doorRequest)
        {
           
            var validUser = await IsUserValidInContext(doorRequest.UserName);
            if (validUser == null || validUser.UserType != UserType.Administrator)
            {
                return this.BadRequest();
            }

            return await this.doorHistoryService.GetDoorHistoryItemsFromCacheAsync(doorRequest.DoorId);
        }
    }
}
