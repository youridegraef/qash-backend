using Microsoft.AspNetCore.Http; // Voeg deze namespace toe
using System.Security.Authentication;
using System.Text.Json;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

public class AccountController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccountController(IUserRepository userRepository, IUserService userService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _userService.Authenticate(model.email, model.password);

            if (user != null)
            {
                Console.WriteLine("Login succesful!");
                
                HttpContext.Session.SetString("LoggedInUser", JsonSerializer.Serialize(user));

                return RedirectToPage("/Index");
            }

            ViewBag.ErrorMessage = "Login failed";
        }

        return View(model);
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _userService.Register(model.name, model.email, model.password, model.dateOfBirth);

            if (user != null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.ErrorMessage = "Register failed";
                return View(model);
            }
        }

        return View(model);
    }
}