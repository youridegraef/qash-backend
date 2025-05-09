using Application.Domain;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Pages;
using Presentation.ViewModels;

namespace Presentation.Pages;

public class IndexModel : BasePageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IUserService _userService;
    private readonly ICategoryService _categoryService;
    private readonly IBudgetService _budgetService;
    private readonly ISavingGoalService _savingGoalService;
    
    public List<BudgetViewModel> Budgets = new List<BudgetViewModel>();
    public List<CategoryViewModel> Categories = new List<CategoryViewModel>();
    public List<SavingGoal> SavingGoals = new List<SavingGoal>();
    public double Balance { get; set; }
    public double Income { get; set; }
    public double Expenses { get; set; }
    [BindProperty] public DateOnly startDate { get; set; }
    [BindProperty] public DateOnly endDate { get; set; }

    public IndexModel(ILogger<IndexModel> logger, IUserService userService, ICategoryService categoryService,
        IBudgetService budgetService, ISavingGoalService savingGoalService)
    {
        _logger = logger;
        _userService = userService;
        _categoryService = categoryService;
        _budgetService = budgetService;
        _savingGoalService = savingGoalService;
    }
    
    public IActionResult OnGet(DateTime? startDate, DateTime? endDate)
    {
        this.startDate = startDate.HasValue
            ? DateOnly.FromDateTime(startDate.Value)
            : DateOnly.FromDateTime(DateTime.Now.AddMonths(-1));
        this.endDate = endDate.HasValue ? DateOnly.FromDateTime(endDate.Value) : DateOnly.FromDateTime(DateTime.Now);

        List<Category> categories = _categoryService.GetByUserId(LoggedInUser.Id);
        SavingGoals = _savingGoalService.GetByUserId(LoggedInUser.Id);

        foreach (var cat in categories)
        {
            var budget = _budgetService.GetByCategoryId(cat.Id);

            if (budget != null)
            {
                Budgets.Add(
                    new BudgetViewModel(
                        cat.Name,
                        cat.Id,
                        _categoryService.CalculateSpendingsByCategoryAndDateRange(cat.Id, this.startDate, this.endDate),
                        budget.Target
                    ));
            }
        }

        Balance = _userService.CalculateBalance(LoggedInUser.Id);
        Income = _userService.CalculateIncomeByDateRange(LoggedInUser.Id, this.startDate, this.endDate);
        Expenses = -_userService.CalculateExpensesByDateRange(LoggedInUser.Id, this.startDate, this.endDate);


        return Page();
    }
}