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
                HttpContext.Session.SetString("LoggedInUser", JsonSerializer.Serialize(user));

                return RedirectToAction("index", "Home");
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

    public IActionResult Profile()
    {
        User? user = GetLoggedInUser();
        ViewBag.User = user;
        
        if (user != null)
        {
            return View(user);
        }

        return RedirectToAction("Login", "Account");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Login", "Account");
    }

    public User? GetLoggedInUser()
    {
        var userJson = HttpContext.Session.GetString("LoggedInUser");

        if (string.IsNullOrEmpty(userJson))
        {
            return null; // Gebruiker is niet ingelogd
        }

        try
        {
            var _user = JsonSerializer.Deserialize<User>(userJson);
            return _user;
        }
        catch (JsonException)
        {
            return null; // Ongeldige JSON in de sessie
        }
    }
}