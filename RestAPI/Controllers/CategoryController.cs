using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
}