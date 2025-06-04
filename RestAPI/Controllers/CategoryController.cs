using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestAPI.RequestModels;
using RestAPI.ResponseModels;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService, ITransactionService transactionService)
    : ControllerBase
{
    [HttpGet("get/{userId:int}")]
    public IActionResult GetByUserId([FromRoute] int userId) {
        var budgets = categoryService.GetByUserId(userId);
        var res = budgets.Select(category =>
            new CategoryResponse(category.Id, category.Name));
        return Ok(res);
    }

    [HttpGet("get-transactions/{categoryId:int}")]
    public IActionResult GetTransactionsByCategory(int categoryId) {
        var transactions = transactionService.GetByCategoryId(categoryId);
        var res = transactions.Select(t =>
            new TransactionResponse(t.Id, t.Description, t.Amount, t.Date, t.Category, t.Tags));
        return Ok(res);
    }

    [HttpPost("add")]
    public IActionResult AddCategory([FromBody] CategoryRequest req) {
        try {
            var category = categoryService.Add(req.Name, req.UserId);
            var res = new CategoryResponse(category.Id, category.Name);
            return Ok(res);
        }
        catch (Exception) {
            return BadRequest(""); //TODO
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteBudget([FromRoute] int id) {
        try {
            var isDeleted = categoryService.Delete(id);

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