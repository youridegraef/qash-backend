using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Presentation.Models;

public class IndexModel : PageModel
{
    public User? LoggedInUser { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var loggedInUserJson = HttpContext.Session.GetString("LoggedInUser");

        if (loggedInUserJson != null)
        {
            LoggedInUser = JsonSerializer.Deserialize<User>(loggedInUserJson);
        }

        if (LoggedInUser == null)
        {
            return RedirectToPage("/account/login");
        }

        return Page();
    }
}