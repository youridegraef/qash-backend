namespace Application.Domain;

public class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; }
    public double Amount { get; set; }
    public DateOnly Date { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }

    public Transaction(double amount, string description, DateOnly date, int userId, int categoryId)
    {
        Description = description;
        Amount = amount;
        Date = date;
        UserId = userId;
    }

    public Transaction(int id, string description, double amount, DateOnly date, int userId, int categoryId)
    {
        Id = id;
        Description = description;
        Amount = amount;
        Date = date;
        UserId = userId;
        CategoryId = categoryId;
    }
}