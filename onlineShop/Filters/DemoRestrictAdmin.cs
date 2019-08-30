using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using onlineShop.Models;
using System.Threading.Tasks;

namespace onlineShop.Filters
{
    // FOR DEMO ONLY
    // Restricts particular actions of admins marked as 'restricted user' in admin panel while preserving general access to it

    public class DemoRestrictAdmin : IAsyncActionFilter
    {
        private readonly UserManager<AppUser> _userManager;

        public DemoRestrictAdmin(
            UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;
            var userDb = await _userManager.GetUserAsync(user);

            // prevent action execution if admin is marked as restricted user
            if (userDb != null && userDb.IsBlocked)
            {
                context.Result = new BadRequestObjectResult("**DEMO: this action is blocked for public admin test account.**");
            }
            else
            {
                await next();
            }
        }
    }
}
