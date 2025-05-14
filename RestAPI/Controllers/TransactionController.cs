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

    [HttpGet("chart-data/{userId}")]
    public IActionResult GetChartData(int userId)
    {
        return Ok(_transactionService.GetChartData(userId));
    }
}