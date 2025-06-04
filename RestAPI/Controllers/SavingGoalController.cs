using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestAPI.RequestModels;
using RestAPI.ResponseModels;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SavingGoalController(ISavingGoalService savingGoalService) : ControllerBase
{
    [HttpPost("add")]
    public IActionResult Add([FromBody] SavingGoalRequest req) {
        try {
            var goal = savingGoalService.Add(req.Name, req.Target, req.Deadline, req.UserId);
            var res = new SavingGoalResponse(goal.Id, goal.Name, goal.AmountSaved, goal.Target, goal.Deadline);
            return Ok(res);
        }
        catch (ArgumentException) {
            return BadRequest("Invalid saving goal data.");
        }
        catch (DatabaseException) {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Een databasefout is opgetreden. Probeer het later opnieuw." });
        }
        catch (Exception) {
            return BadRequest("An unexpected error occured.");
        }
    }

    [HttpGet("get/{userId:int}")]
    public IActionResult Get([FromRoute] int userId) {
        var savingGoals = savingGoalService.GetByUserId(userId);
        var res = savingGoals.Select(g =>
            new SavingGoalResponse(g.Id, g.Name, g.AmountSaved, g.Target, g.Deadline));
        return Ok(res);
    }

    [HttpPut("edit/{userId:int}")]
    public IActionResult Edit([FromRoute] int userId, [FromBody] SavingGoalRequest req) {
        throw new NotImplementedException();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteBudget([FromRoute] int id) {
        try {
            var isDeleted = savingGoalService.Delete(id);

            if (!isDeleted) {
                throw new Exception();
            }

            return Ok("Successfully deleted!");
        }
        catch (ArgumentException) {
            return BadRequest("Invalid saving goal data.");
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