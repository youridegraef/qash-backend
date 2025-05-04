using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Pages;

namespace Presentation.Pages.SavingGoals;

public class Index : BasePageModel
{
    public IActionResult OnGet()
    {
        return Page();
    }
}