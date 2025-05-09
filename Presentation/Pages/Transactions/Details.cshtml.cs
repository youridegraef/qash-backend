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
    
    [BindProperty]
    public Transaction transaction { get; set; }
    public Category category { get; set; }
    public List<Category> Categories { get; set; }

    public IActionResult OnGet(int id)
    {
        transaction = _transactionService.GetById(id);
        var categories = _categoryService.GetAll();
        Categories = categories;
        category = categories.FirstOrDefault(c => c.Id == transaction.CategoryId);

        
        return Page();
    }
}