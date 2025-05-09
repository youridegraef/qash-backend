using Application.Domain;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Presentation.Pages;

public abstract class BasePageModel : PageModel
{
    protected User LoggedInUser { get; private set; }

    private static readonly HashSet<string> AnonymousPages = new()
    {
        "/Account/Login",
        "/Account/Register",
        "/Account/ForgotPassword"
    };

    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        base.OnPageHandlerExecuting(context);

        var currentPath = context.HttpContext.Request.Path.Value ?? "";

        if (AnonymousPages.Contains(currentPath))
        {
            return;
        }

        LoggedInUser = GetLoggedInUserFromSession();

        if (LoggedInUser == null)
        {
            var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;

            context.Result = new RedirectToPageResult("/Account/Login", new { returnUrl });
        }
    }

    protected User GetLoggedInUserFromSession()
    {
        try
        {
            if (HttpContext.Session == null)
            {
                return null;
            }

            var userJson = HttpContext.Session.GetString("LoggedInUser");
            var LoggedInUser = JsonSerializer.Deserialize<User>(userJson);

            if (!string.IsNullOrEmpty(userJson))
            {
                ViewData["User"] = LoggedInUser;
                return JsonSerializer.Deserialize<User>(userJson);
            }
            
            return new User { Id = LoggedInUser.Id };
        }
        catch
        {
            return null;
        }
    }
}