using Microsoft.AspNetCore.Http; // Voeg deze namespace toe
using System.Security.Authentication;
using System.Text.Json;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

public class AccountController : BaseController
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
        return View(LoggedInUser);
    }

    public IActionResult EditProfile()
    {
        ProfileModel viewModel = new ProfileModel
        {
            email = LoggedInUser.Email,
            name = LoggedInUser.Name,
            dateOfBirth = LoggedInUser.DateOfBirth
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(ProfileModel model)
    {
        if (ModelState.IsValid)
        {
            if (LoggedInUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            LoggedInUser.Name = model.name;
            LoggedInUser.Email = model.email;
            LoggedInUser.DateOfBirth = model.dateOfBirth;

            var isUserEdited = _userService.Update(LoggedInUser);

            if (isUserEdited)
            {
                HttpContext.Session.SetString("LoggedInUser", JsonSerializer.Serialize(LoggedInUser));
                return RedirectToAction("Profile");
            }
            else
            {
                ViewBag.ErrorMessage = "Editing profile failed";
                return View(model);
            }
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

            return View(model);
        }

        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Login", "Account");
    }
}