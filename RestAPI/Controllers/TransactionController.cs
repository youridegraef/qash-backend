using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("balance/{userId}")]
    public IActionResult GetBalance(int userId)
    {
        return Ok(_transactionService.GetBalance(userId));
    }

    [HttpGet("income/{userId}")]
    public IActionResult GetIncome(int userId)
    {
        return Ok(_transactionService.GetIncome(userId));
    }

    [HttpGet("expenses/{userId}")]
    public IActionResult GetExpenses(int userId)
    {
        return Ok(_transactionService.GetExpenses(userId));
    }

    [HttpGet("chart-data/{userId}")]
    public IActionResult GetChartData(int userId)
    {
        return Ok(_transactionService.GetChartData(userId));
    }
}