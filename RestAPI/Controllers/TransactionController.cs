using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController(ITransactionService transactionService, ITagService tagService) : ControllerBase
{
    [HttpGet("balance/{userId}")]
    public IActionResult GetBalance([FromRoute] int userId)
    {
        return Ok(transactionService.GetBalance(userId));
    }

    [HttpGet("income/{userId}")]
    public IActionResult GetIncome([FromRoute] int userId)
    {
        return Ok(transactionService.GetIncome(userId));
    }

    [HttpGet("expenses/{userId}")]
    public IActionResult GetExpenses([FromRoute] int userId)
    {
        return Ok(transactionService.GetExpenses(userId));
    }

    [HttpPost("/add/{userId}")]
    public IActionResult AddTransaction([FromBody] Requests.AddTransaction req, [FromRoute] int userId)
    {
        try
        {
            var transaction = transactionService.Add(req.Description, req.Amount, req.Date, userId, req.Category.Id);
            transaction.Tags = tagService.GetByTransactionId(transaction.Id);

            TransactionModel res = new TransactionModel(transaction.Id, transaction.Description, transaction.Amount,
                transaction.Date, transaction.Category, userId, transaction.Tags);
            
            //TODO: Tags moeten ook in de database worden gezet (koppeltabel)
            
            return Ok();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpGet("get/{userId}")]
    public IActionResult GetTrasactionsPaged([FromRoute] int userId,
        [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        if (page != null && pageSize != null)
        {
            var paged = transactionService.GetByUserIdPaged(userId, page.Value, pageSize.Value);
            return Ok(paged);
        }
        else
        {
            var all = transactionService.GetByUserId(userId);
            return Ok(all);
        }
    }
}