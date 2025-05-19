using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController(ITransactionService transactionService) : ControllerBase
{
    [HttpGet("balance/{userId}")]
    public IActionResult GetBalance(int userId)
    {
        return Ok(transactionService.GetBalance(userId));
    }

    [HttpGet("income/{userId}")]
    public IActionResult GetIncome(int userId)
    {
        return Ok(transactionService.GetIncome(userId));
    }

    [HttpGet("expenses/{userId}")]
    public IActionResult GetExpenses(int userId)
    {
        return Ok(transactionService.GetExpenses(userId));
    }

    [HttpGet("chart-data/{userId}")]
    public IActionResult GetChartData(int userId)
    {
        return Ok(transactionService.GetChartData(userId));
    }
}