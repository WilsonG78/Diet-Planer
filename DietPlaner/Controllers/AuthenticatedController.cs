using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DietPlaner.Controllers;

public class AuthenticatedController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            context.Result = RedirectToAction("Login", "Account");
            return;
        }
        base.OnActionExecuting(context);
    }

    protected bool IsAdmin() =>
        HttpContext.Session.GetString("UserRole") == "Admin";
}
