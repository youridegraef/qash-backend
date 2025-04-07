using Application.Domain;

namespace Presentation.Models;

public class TransactionModel
{
    public int Id { get; set; }
    public double Amount { get; set; }
    public DateOnly Date { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public List<Category> Categories { get; set; }
}