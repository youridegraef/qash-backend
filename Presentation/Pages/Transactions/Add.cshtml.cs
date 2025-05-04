using Application.Domain;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Presentation.Pages.Transactions;

public class Add : BasePageModel
{
    private readonly ITransactionService _transactionService;
    private readonly ICategoryService _categoryService;

    public Add(ITransactionService transactionService,
        ICategoryService categoryService)
    {
        _transactionService = transactionService;
        _categoryService = categoryService;
    }
    
    [BindProperty] public double Amount { get; set; }
    [BindProperty] public DateOnly Date { get; set; }
    [BindProperty] public int UserId { get; set; }
    [BindProperty] public int CategoryId { get; set; }
    [BindProperty] public List<Category> Categories { get; set; }


    public IActionResult OnGet()
    {
        var categories = _categoryService.GetALl();

        Categories = categories;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var transaction = _transactionService.Add(Amount, Date, LoggedInUser.Id, CategoryId);

            if (transaction != null)
            {
                return RedirectToPage("/../index");
            }
            else
            {
                ViewData["ErrorMessage"] = "Failed to add transaction.";
                return Page();
            }
        }

        return Page();
    }
}