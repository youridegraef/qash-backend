using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Pages;

namespace Presentation.Pages;

public class IndexModel : BasePageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IUserService _userService;

    public IndexModel(ILogger<IndexModel> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    public double Balance { get; set; }
    public double Income { get; set; }
    public double Expenses { get; set; }
    [BindProperty] public DateOnly startDate { get; set; }
    [BindProperty] public DateOnly endDate { get; set; }


    public IActionResult OnGet(DateTime? startDate, DateTime? endDate)
    {
        this.startDate = startDate.HasValue ? DateOnly.FromDateTime(startDate.Value) : DateOnly.FromDateTime(DateTime.Now.AddMonths(-1));
        this.endDate = endDate.HasValue ? DateOnly.FromDateTime(endDate.Value) : DateOnly.FromDateTime(DateTime.Now);

        Balance = _userService.CalculateBalance(LoggedInUser.Id);
        Income = _userService.CalculateIncome(LoggedInUser.Id);
        Expenses = _userService.CalculateExpenses(LoggedInUser.Id);

        return Page();
    }
}