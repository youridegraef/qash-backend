using Application.Domain;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

public class SavingGoalController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserService _userService;
    private readonly ISavingGoalService _savingGoalService;

    public SavingGoalController(ILogger<HomeController> logger, IUserService userService,
        ISavingGoalService savingGoalService)
    {
        _logger = logger;
        _userService = userService;
        _savingGoalService = savingGoalService;
    }

    public IActionResult Index()
    {
        List<SavingGoal> savingGoals = _savingGoalService.GetAll();

        SavingGoalsModel viewModel = new SavingGoalsModel
        {
            SavingGoals = savingGoals
        };
        
        return View(viewModel);
    }
    
    public IActionResult Add()
    {
        throw new NotImplementedException();
    }
    
    public IActionResult Details()
    {
        throw new NotImplementedException();
    }
    
    public IActionResult Edit(int id)
    {
        var savingGoal = _savingGoalService.GetById(id);
        
        SavingGoalModel viewModel = new SavingGoalModel
        {
            Id = savingGoal.Id,
            Target = savingGoal.Target,
            Deadline = savingGoal.Deadline,
            UserId = savingGoal.UserId,
        };

        return View(viewModel);
    }
    
    public IActionResult Remove()
    {
        throw new NotImplementedException();
    }
}