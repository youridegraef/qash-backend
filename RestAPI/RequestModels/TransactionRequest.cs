using Application.Domain;

namespace RestAPI.RequestModels;

public class TransactionRequest(
    int id,
    string description,
    double amount,
    DateOnly date,
    Category category,
    int userId,
    List<Tag> tags)
{
    public int Id { get; set; } = id;
    public string Description { get; set; } = description;
    public double Amount { get; set; } = amount;
    public DateOnly Date { get; set; } = date;
    public Category Category { get; set; } = category;
    public int UserId { get; set; } = userId;
    public List<Tag> Tags { get; set; } = tags;
}