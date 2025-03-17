using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Presentation.Pages;

public class Register : PageModel
{
    private readonly IUserService _userService;

    public string ErrorMessage { get; set; }
    public User CreatedUser { get; set; }

    public Register(IUserService userService)
    {
        _userService = userService;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public IActionResult OnPost(string name, string email, string password, DateOnly dateOfBirth)
    {
        try
        {
            CreatedUser = _userService.Register(name, email, password, dateOfBirth);
        }
        catch (InvalidDataException ex)
        {
            ErrorMessage = ex.Message;
        }

        return Page();
    }
}