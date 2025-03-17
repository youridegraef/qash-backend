namespace Application.Domain;

public class Transaction
{
    public int Id { get; private set; }
    public double Amount { get; set; }
    public DateOnly Date { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }

    public List<Tag> Tags { get; set; } = new List<Tag>();

    public Transaction(int id, double amount, DateOnly date, int userId, int categoryId)
    {
        Id = id;
        Amount = amount;
        Date = date;
        UserId = userId;
        CategoryId = categoryId;
    }

    public Transaction(int id, double amount, DateOnly date, List<Tag> tags, int userId)
    {
        Id = id;
        Amount = amount;
        Date = date;
        Tags = tags;
        UserId = userId;
    }

    public Transaction(double amount, DateOnly date)
    {
        Amount = amount;
        Date = date;
    }
}