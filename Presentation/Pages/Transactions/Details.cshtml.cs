using Application.Domain;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Presentation.Pages.Transactions;

public class Details : BasePageModel
{
    private readonly ITransactionService _transactionService;
    private readonly ICategoryService _categoryService;

    public Details(ITransactionService transactionService,
        ICategoryService categoryService)
    {
        _transactionService = transactionService;
        _categoryService = categoryService;
    }

    [BindProperty] public int Id { get; set; }
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
}