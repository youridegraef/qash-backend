using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestAPI.RequestModels;
using RestAPI.ResponseModels;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetController(IBudgetService budgetService) : ControllerBase
{
    [HttpGet("get/{userId:int}")]
    public IActionResult GetByUserId([FromRoute] int userId) {
        var budgets = budgetService.GetByUserId(userId);
        var res = budgets.Select(budget =>
            new BudgetResponse(budget.Id, budget.Name, budget.StartDate, budget.EndDate, budget.Spent, budget.Target,
                userId));

        return Ok(res);
    }

    [HttpPost("add")]
    public IActionResult AddBudget([FromBody] BudgetRequest req) {
        try {
            if (req.Target < 0) {
                return BadRequest("Target can't be less than 0.");
            }

            var budget = budgetService.Add(req.StartDate, req.EndDate, req.Target, req.CategoryId);

            var res = new BudgetResponse(budget.Id, budget.Name, budget.StartDate, budget.EndDate, budget.Spent,
                budget.Target, req.UserId);

            return Ok(res);
        }
        catch (ArgumentException) {
            return BadRequest("Invalid budget data.");
        }
        catch (DatabaseException) {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Een databasefout is opgetreden. Probeer het later opnieuw." });
        }
        catch (Exception) {
            return BadRequest("An unexpected error occured.");
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteBudget([FromRoute] int id) {
        try {
            var isDeleted = budgetService.Delete(id);

            if (!isDeleted) {
                throw new Exception();
            }

            return Ok("Successfully deleted!");
        }
        catch (ArgumentException) {
            return BadRequest("Invalid budget data.");
        }
        catch (DatabaseException) {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Een databasefout is opgetreden. Probeer het later opnieuw." });
        }
        catch (Exception) {
            return BadRequest("An unexpected error occured.");
        }
    }
}