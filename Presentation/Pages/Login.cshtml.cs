using System.Security.Authentication;
using Application.Interfaces;
using Application.Services;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Presentation.Pages;

public class Login : PageModel
{
    private readonly IUserService _userService;

    public string ErrorMessage { get; set; }
    public User LoggedInUser { get; set; }
    
    public Login(IUserService userService)
    {
        _userService = userService;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public IActionResult OnPost(string email, string password)
    {
        try
        {
            LoggedInUser = _userService.AuthenticateUser(email, password);
            if (LoggedInUser != null)
            {
                return Page();
            }
        }
        catch (AuthenticationException ex)
        {
            ErrorMessage = ex.Message;
        }
        return Page();
    }
}