
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace API.Helper
{
    public class LogUserActivity:IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // execute any code before the action executes
            var resultContext = await next();
            // execute any code after the action executes\

            var userName=resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo.GetUserByUsernameAsync(userName);

            user.LastActive = DateTime.Now.ToUniversalTime();

            await repo.SaveAllAsync();


        }


    }
}
