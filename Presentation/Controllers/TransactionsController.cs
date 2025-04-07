using System.Text.Json;
using Application.Domain;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

public class TransactionsController : BaseController
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
        var transactions = _transactionService.GetAll();
        var categories = _categoryService.GetALl();

        var viewModel = new TransactionsModel
        {
            Transactions = transactions,
            Categories = categories
        };

        return View(viewModel);
    }

    public IActionResult Edit(int id)
    {
        var transaction = _transactionService.GetById(id);
        var categories = _categoryService.GetALl();
        
        TransactionModel viewModel = new TransactionModel
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Date = transaction.Date,
            UserId = transaction.UserId,
            CategoryId = transaction.CategoryId,
            Categories = categories
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(TransactionModel model)
    {
        if (ModelState.IsValid)
        {
            Transaction newTransaction =
                new Transaction(model.Id, model.Amount, model.Date, (int)LoggedInUser.Id, model.CategoryId);

            bool isEdited = _transactionService.Edit(newTransaction);
            if (isEdited)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = "Editing transaction failed";
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

    public IActionResult Details(int id)
    {
        var transaction = _transactionService.GetById(id);
        var categories = _categoryService.GetALl();

        TransactionModel viewModel = new TransactionModel
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Date = transaction.Date,
            UserId = transaction.UserId,
            CategoryId = transaction.CategoryId,
            Categories = categories
        };

        return View(viewModel);
    }

    public IActionResult Add()
    {
        var categories = _categoryService.GetALl();

        TransactionModel viewModel = new TransactionModel
        {
            Categories = categories
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Add(TransactionModel model)
    {
        if (ModelState.IsValid)
        {
            var transaction = _transactionService.Add(model.Amount, model.Date, (int)LoggedInUser.Id, model.CategoryId);

            if (transaction != null)
            {
                return RedirectToAction("index");
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to add transaction.";
                return View(model);
            }
        }

        return View(model);
    }

    public IActionResult Remove(int id)
    {
        bool isDeleted = _transactionService.Delete(id);

        if (isDeleted)
        {
            ViewBag.SuccesMessage = "Transaction has succesfully been deleted.";
            return RedirectToAction("Index");
        }

        ViewBag.ErrorMessage = "Transaction has not been deleted";
        return RedirectToAction("Index");
    }
}