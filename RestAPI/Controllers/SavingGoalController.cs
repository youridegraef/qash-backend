using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestAPI.RequestModels;
using RestAPI.ResponseModels;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SavingGoalController(ISavingGoalService savingGoalService) : ControllerBase
{
    [HttpPost("/add/{userId:int}")]
    public IActionResult Add([FromRoute] int userId, [FromBody] SavingGoalRequest req) {
        throw new NotImplementedException();
    }

    [HttpPost("/get/{userId:int}")]
    public IActionResult Get([FromRoute] int userId) {
        var savingGoals = savingGoalService.GetByUserId(userId);
        var res = savingGoals.Select(g =>
            new SavingGoalResponse(g.Id, g.Name, g.AmountSaved, g.Target, g.Deadline));
        return Ok(res);
    }

    [HttpPut("/edit/{userId:int}")]
    public IActionResult Edit([FromRoute] int userId, [FromBody] SavingGoalRequest req) {
        throw new NotImplementedException();
    }
}