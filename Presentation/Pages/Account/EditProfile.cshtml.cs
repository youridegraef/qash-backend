using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Pages;

namespace Presentation.Pages.Account;

public class EditProfile : BasePageModel
{
    public IActionResult OnGet()
    {
        return Page();
    }
}