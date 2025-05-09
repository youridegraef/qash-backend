using Application.Domain;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.ViewModels;

namespace Presentation.Pages.Transactions;

public class Edit : BasePageModel
{
    private readonly ITransactionService _transactionService;
    private readonly ICategoryService _categoryService;

    public Edit(ITransactionService transactionService,
        ICategoryService categoryService)
    {
        _transactionService = transactionService;
        _categoryService = categoryService;
    }

    [BindProperty] public TransactionViewModel Transaction { get; set; }
    public CategoryViewModel Category { get; set; }
    [BindProperty] public List<CategoryViewModel> Categories { get; set; }


    public IActionResult OnGet(int id)
    {
        Transaction transaction = _transactionService.GetById(id);
        Transaction = new TransactionViewModel(transaction.Id, transaction.Description, Transaction.Amount,
            Transaction.Date,
            Transaction.CategoryId);

        List<Category> categories = _categoryService.GetAll();
        foreach (var cat in categories)
        {
            Categories.Add(
                new CategoryViewModel(
                    cat.Name,
                    cat.Id
                ));
        }

        Category category = categories.FirstOrDefault(c => c.Id == Transaction.CategoryId);
        Category = new CategoryViewModel(category.Name, category.Id);

        return Page();
    }

    public IActionResult OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            bool isAdded = _transactionService.Edit(Transaction.Id, Transaction.Amount, Transaction.Description,
                Transaction.Date,
                LoggedInUser.Id, Transaction.CategoryId);

            if (isAdded)
            {
                return RedirectToPage("/index");
            }

            ViewData["ErrorMessage"] = "Failed to add transaction.";
        }

        return Page();
    }
}