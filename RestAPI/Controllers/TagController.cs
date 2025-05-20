using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using RestAPI.RequestModels;
using RestAPI.ResponseModels;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController(ITagService tagService) : ControllerBase
{
    [HttpGet("get/{userId}")]
    public IActionResult Get([FromRoute] int userId, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        if (page != null && pageSize != null)
        {
            var tags = tagService.GetByUserIdPaged(userId, page.Value, pageSize.Value);
            var res = tags.Select(tag => new TagResponse(tag.Id, tag.Name, tag.ColorHexCode)).ToList();

            return Ok(res);
        }
        else
        {
            var tags = tagService.GetByUserId(userId);
            var res = tags.Select(tag => new TagResponse(tag.Id, tag.Name, tag.ColorHexCode)).ToList();

            return Ok(res);
        }
    }

    [HttpPost("add/{userId}")]
    public IActionResult Add([FromRoute] int userId, [FromBody] SavingGoalRequest req)
    {
        try
        {
            var tag = tagService.Add(req.Name, req.ColorHexCode, userId);
            var res = new TagResponse(tag.Id, tag.Name, tag.ColorHexCode);
            return Ok(res);
        }
        catch (Exception)
        {
            return BadRequest(); //TODO: Check welke exception te gooien
        }
    }
}