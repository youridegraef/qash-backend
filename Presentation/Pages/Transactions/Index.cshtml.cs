using Application.Domain;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Pages;
using Presentation.ViewModels;

namespace Presentation.Pages.Transactions;

public class TransactionsModel : BasePageModel
{
    private readonly ILogger<TransactionsModel> _logger;
    private readonly ITransactionService _transactionService;
    private readonly ICategoryService _categoryService;

    public TransactionsModel(ILogger<TransactionsModel> logger,
        ITransactionService transactionService, ICategoryService categoryService)
    {
        _logger = logger;
        _transactionService = transactionService;
        _categoryService = categoryService;
    }

    public List<TransactionViewModel> Transactions = new List<TransactionViewModel>();
    public List<CategoryViewModel> Categories = new List<CategoryViewModel>();

    public IActionResult OnGet()
    {
        List<Transaction> transactions = _transactionService.GetByUserId(LoggedInUser.Id);
        transactions = transactions.OrderByDescending(t => t.Date).ToList();
        foreach (var transaction in transactions)
        {
            Transactions.Add(
                new TransactionViewModel(
                    transaction.Id,
                    transaction.Description,
                    transaction.Amount,
                    transaction.Date,
                    transaction.CategoryId
                )
            );
        }

        List<Category> categories = _categoryService.GetByUserId(LoggedInUser.Id);
        foreach (var category in categories)
        {
            Categories.Add(
                new CategoryViewModel(
                    category.Name,
                    category.Id
                ));
        }

        return Page();
    }
}