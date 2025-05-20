using Application.Domain;

namespace Application.Dtos;

public class TransactionDto(
    int id,
    string description,
    double amount,
    DateOnly date,
    int userId,
    CategoryDto category
)
{
    public int Id { get; set; } = id;
    public string Description { get; set; } = description;
    public double Amount { get; set; } = amount;
    public DateOnly Date { get; set; } = date;
    public int UserId { get; set; } = userId;
    public CategoryDto Category { get; set; } = category;
    public ICollection<TagDto> Tags { get; set; }
}