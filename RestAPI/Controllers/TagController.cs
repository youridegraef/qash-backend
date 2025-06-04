using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestAPI.RequestModels;
using RestAPI.ResponseModels;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController(ITagService tagService) : ControllerBase
{
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

    [HttpPost("add")]
    public IActionResult Add([FromBody] TagRequest req) {
        try {
            var tag = tagService.Add(req.Name, req.UserId);
            var res = new TagResponse(tag.Id, tag.Name);
            return Ok(res);
        }
        catch (Exception) {
            return BadRequest(""); //TODO
        }
    }

    [HttpPut("/edit")]
    public IActionResult Edit([FromBody] TagRequest req) {
        throw new NotImplementedException();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteBudget([FromRoute] int id) {
        try {
            var isDeleted = tagService.Delete(id);

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