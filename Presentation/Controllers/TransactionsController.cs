using System.Text.Json;
using System.Transactions;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

public class TransactionsController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITransactionService _transactionService;
    private readonly ICategoryService _categoryService;

    public TransactionsController(ILogger<HomeController> logger, ITransactionService transactionService,
        ICategoryService categoryService)
    {
        _logger = logger;
        _transactionService = transactionService;
        _categoryService = categoryService;
    }

    public IActionResult Index()
    {
        User? user = GetLoggedInUser();
        ViewBag.User = user;

        if (user != null)
        {
            var transactions = _transactionService.GetAll();
            var categories = _categoryService.GetByUserId((int)user.Id);

            var viewModel = new TransactionsModel
            {
                Transactions = transactions,
                Categories = categories
            };

            return View(viewModel);
        }

        return RedirectToAction("Login", "Account");
    }

    public IActionResult Edit()
    {
        throw new NotImplementedException();
    }

    public IActionResult Details()
    {
        throw new NotImplementedException();
    }

    public IActionResult Add()
    {
        User? user = GetLoggedInUser();
        ViewBag.User = user;

        if (user != null)
        {
            return View();
        }

        return RedirectToAction("Login", "Account");
    }

    public IActionResult Remove()
    {
        throw new NotImplementedException();
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