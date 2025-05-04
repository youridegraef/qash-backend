using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Pages;

namespace Presentation.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public string ErrorMessage { get; set; }

    public LoginModel(IUserService userService, IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    [BindProperty]
    [Required]
    [EmailAddress]
    public string email { get; set; }

    [BindProperty] [Required] public string password { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public IActionResult OnPost()
    {
        if (ModelState.IsValid)
        {
            var user = _userService.Authenticate(email, password);

            if (user != null)
            {
                HttpContext.Session.SetString("LoggedInUser", JsonSerializer.Serialize(user));

                return RedirectToPage("../Index");
            }

            ErrorMessage = "Invalid email or password";
        }

        if (!ModelState.IsValid)
        {
            foreach (var modelStateKey in ModelState.Keys)
            {
                var modelStateVal = ModelState[modelStateKey];
                foreach (var error in modelStateVal.Errors)
                {
                    var errorMessage = error.ErrorMessage;
                    // Of error.Exception, om de daadwerkelijke uitzondering te krijgen
                    Console.WriteLine($"Key: {modelStateKey}, Error: {errorMessage}");
                }
            }

            return Page();
        }

        return Page();
    }
}