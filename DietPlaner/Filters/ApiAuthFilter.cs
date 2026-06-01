using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DietPlaner.Data;

namespace DietPlaner.Filters;

public class ApiAuthFilter : IActionFilter
{
    private readonly DietPlanerContext _context;

    public ApiAuthFilter(DietPlanerContext context)
    {
        _context = context;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var username = context.HttpContext.Request.Headers["X-Username"].FirstOrDefault();
        var token = context.HttpContext.Request.Headers["X-Api-Token"].FirstOrDefault();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                error = "Missing X-Username or X-Api-Token header."
            });
            return;
        }

        var user = _context.User.FirstOrDefault(u => u.LoginName == username && u.ApiToken == token);
        if (user == null)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                error = "Invalid username or token."
            });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
