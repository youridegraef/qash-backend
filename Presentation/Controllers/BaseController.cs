using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class BaseController : Controller
{
    protected User? LoggedInUser { get; private set; }

    private static readonly List<string> AnonymousActions = new List<string>
    {
        "Login",
        "CreateAccount"
    };

    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        if (!AnonymousActions.Contains(context.ActionDescriptor.RouteValues["action"]))
        {
            LoggedInUser = GetLoggedInUser();
            ViewBag.User = LoggedInUser;

            if (LoggedInUser == null)
            {
                context.Result = RedirectToAction("Login", "Account");
                return;
            }
        }
        else
        {
            LoggedInUser = GetLoggedInUser();
            ViewBag.User = LoggedInUser;
        }

        base.OnActionExecuting(context);
    }

    private User? GetLoggedInUser()
    {
        var userJson = HttpContext.Session.GetString("LoggedInUser");

        if (string.IsNullOrEmpty(userJson))
        {
            return null;
        }

        try
        {
            var _user = JsonSerializer.Deserialize<User>(userJson);
            return _user;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}