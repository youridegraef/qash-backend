using Application.Domain;

namespace RestAPI.RequestModels;

public class TransactionRequest(
    string description,
    double amount,
    DateOnly date,
    int categoryId,
    int userId,
    List<int> tagIds) {
    public string Description { get; set; } = description;
    public double Amount { get; set; } = amount;
    public DateOnly Date { get; set; } = date;
    public int CategoryId { get; set; } = categoryId;
    public int UserId { get; set; } = userId;
    public List<int> TagIds { get; set; } = tagIds;
}