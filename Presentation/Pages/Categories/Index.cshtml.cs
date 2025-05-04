using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Pages;

namespace Presentation.Pages.Categories;

public class Index : BasePageModel
{
    public IActionResult OnGet()
    {
        return Page();
    }
}