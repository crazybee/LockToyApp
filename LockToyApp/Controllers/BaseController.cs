using LockToyApp.Models;
using LockToyApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LockToyApp.Controllers
{
    public class BaseController : ControllerBase
    {
        public readonly IUserService userService;

        protected BaseController(IUserService userService)
        {
            this.userService = userService;
        }
        protected async Task<DBEntities.User> IsUserValidInContext(string userName)
        {
            var user = await this.userService.GetUserByName(userName);
            var userInContext = this.HttpContext.Items["User"] as DBEntities.User;
            if (user == null || userInContext == null || user != userInContext)
            {
                return null;
            }
            return user;
        }
    }
}
