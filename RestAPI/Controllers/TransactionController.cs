using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestAPI.RequestModels;
using RestAPI.ResponseModels;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController(ITransactionService transactionService, ITagService tagService) : ControllerBase
{
    [HttpGet("balance/{userId}")]
    public IActionResult GetBalance([FromRoute] int userId) {
        return Ok(transactionService.GetBalance(userId));
    }

    [HttpGet("income/{userId}")]
    public IActionResult GetIncome([FromRoute] int userId) {
        return Ok(transactionService.GetIncome(userId));
    }

    [HttpGet("expenses/{userId}")]
    public IActionResult GetExpenses([FromRoute] int userId) {
        return Ok(transactionService.GetExpenses(userId));
    }

    [HttpPost("add")]
    public IActionResult AddTransaction([FromBody] TransactionRequest req) {
        try {
            var tags = new List<Tag>();
            foreach (var tagId in req.TagIds) {
                tags.Add(tagService.GetById(tagId));
            }

            var transaction =
                transactionService.Add(req.Description, req.Amount, req.Date, req.UserId, req.CategoryId, tags);

            var res = new TransactionResponse(transaction.Id, transaction.Description, transaction.Amount,
                transaction.Date, transaction.Category, transaction.Tags);

            return Ok(res);
        }
        catch (ArgumentException) {
            return BadRequest("Invalid transaction data.");
        }
        catch (DatabaseException) {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Een databasefout is opgetreden. Probeer het later opnieuw." });
        }
        catch (Exception) {
            return BadRequest("An unexpected error occured.");
        }
    }

    [HttpGet("get/{userId}")]
    public IActionResult GetTransactionsPaged([FromRoute] int userId,
        [FromQuery] int? page,
        [FromQuery] int? pageSize) {
        if (page != null && pageSize != null) {
            var transactions = transactionService.GetByUserIdPaged(userId, page.Value, pageSize.Value);
            var res = transactions.Select(transaction => new TransactionResponse(transaction.Id,
                transaction.Description, transaction.Amount, transaction.Date, transaction.Category,
                transaction.Tags)).ToList();

            return Ok(res);
        }
        else {
            var transactions = transactionService.GetByUserId(userId);
            var res = transactions.Select(transaction => new TransactionResponse(transaction.Id,
                transaction.Description, transaction.Amount, transaction.Date, transaction.Category,
                transaction.Tags)).ToList();

            return Ok(res);
        }
    }

    [HttpPut("/edit/{userId:int}")]
    public IActionResult Edit([FromRoute] int userId, [FromBody] TransactionRequest req) {
        throw new NotImplementedException();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteTransaction([FromRoute] int id) {
        try {
            var isDeleted = transactionService.Delete(id);

            if (!isDeleted) {
                throw new Exception();
            }

            return Ok("Successfully deleted!");
        }
        catch (ArgumentException) {
            return BadRequest("Invalid transaction data.");
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