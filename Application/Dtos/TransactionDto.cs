using Application.Domain;

namespace Application.Dtos;

public class TransactionDto(
    int id,
    string description,
    double amount,
    DateOnly date,
    int userId,
    int categoryId,
    Category category,
    List<Tag> tags) {
    public int Id { get; set; } = id;
    public string Description { get; set; } = description;
    public double Amount { get; set; } = amount;
    public DateOnly Date { get; set; } = date;
    public int UserId { get; set; } = userId;
    public int CategoryId { get; set; } = categoryId;
    public Category Category { get; set; } = category;
    public List<Tag> Tags { get; set; } = tags;
}