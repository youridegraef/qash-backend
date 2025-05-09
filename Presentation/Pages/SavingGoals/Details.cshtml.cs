using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Presentation.Pages.SavingGoals;

public class Details : BasePageModel
{
    public IActionResult OnGet(int id)
    {
        return Page();
    }
}