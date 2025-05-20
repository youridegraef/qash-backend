using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestAPI.RequestModels;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SavingGoalController(ISavingGoalService savingGoalService) : ControllerBase
{
    [HttpPost("/add/{userId:int}")]
    public IActionResult Add([FromRoute] int userId, [FromBody] SavingGoalRequest req)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("/get/{userId:int}")]
    public IActionResult Get([FromRoute] int userId)
    {
        throw new NotImplementedException();
    }
}