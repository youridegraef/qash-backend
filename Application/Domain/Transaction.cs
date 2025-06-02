namespace Application.Domain;

public class Transaction {
    public int Id { get; private set; }
    public string Description { get; private set; }
    public double Amount { get; private set; }
    public DateOnly Date { get; private set; }
    public int UserId { get; private set; }
    public int CategoryId { get; private set; }

    public Transaction(double amount, string description, DateOnly date, int userId, int categoryId) {
        Description = description;
        Amount = amount;
        Date = date;
        UserId = userId;
    }

    public Transaction(int id, string description, double amount, DateOnly date, int userId, int categoryId) {
        Id = id;
        Description = description;
        Amount = amount;
        Date = date;
        UserId = userId;
        CategoryId = categoryId;
    }
}