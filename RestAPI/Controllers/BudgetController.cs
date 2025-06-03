using Application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RestAPI.ResponseModels;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetController(IBudgetService budgetService, ICategoryService categoryService) : ControllerBase
{
    [HttpGet("get/{userId:int}")]
    public IActionResult GetByUserId([FromRoute] int userId) {
        var budgets = budgetService.GetByUserId(userId);
        var res = budgets.Select(budget =>
            new BudgetResponse(budget.Id, budget.Name, budget.Spent, budget.Target, userId));

        return Ok(res);
    }
}