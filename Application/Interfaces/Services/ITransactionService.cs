using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITransactionService
{
    public Transaction GetById(int id);
    public List<Transaction> GetByUserId(int userId);
    public List<Transaction> GetByUserIdPaged(int userId, int page, int pageSize);
    public Transaction Add(string description, double amount, DateOnly date, int userId, int categoryId);
    public bool Edit(int id, double amount, string description, DateOnly date, int userId, int categoryId);
    public bool Delete(int id);
    public double GetBalance(int userId);
    public double GetExpenses(int userId);
    public double GetIncome(int userId);
}