using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using RestAPI.RequestModels;
using RestAPI.ResponseModels;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController(ITagService tagService) : ControllerBase {
    [HttpGet("get/{userId:int}")]
    public IActionResult Get([FromRoute] int userId, [FromQuery] int? page, [FromQuery] int? pageSize) {
        if (page != null && pageSize != null) {
            var tags = tagService.GetByUserIdPaged(userId, page.Value, pageSize.Value);
            var res = tags.Select(tag => new TagResponse(tag.Id, tag.Name)).ToList();

            return Ok(res);
        }
        else {
            var tags = tagService.GetByUserId(userId);
            var res = tags.Select(tag => new TagResponse(tag.Id, tag.Name)).ToList();

            return Ok(res);
        }
    }

    [HttpPost("add/{userId:int}")]
    public IActionResult Add([FromRoute] int userId, [FromBody] SavingGoalRequest req) {
        try {
            var tag = tagService.Add(req.Name, userId);
            var res = new TagResponse(tag.Id, tag.Name);
            return Ok(res);
        }
        catch (Exception) {
            return BadRequest("");
        }
    }

    [HttpPut("/edit/{userId:int}")]
    public IActionResult Edit([FromRoute] int userId, [FromBody] TagRequest req) {
        throw new NotImplementedException();
    }
}