using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITransactionService
{
    public Transaction GetById(int id);
    public List<Transaction> GetAll();
    public List<Transaction> GetByDateRange(DateOnly startDate, DateOnly endDate);
    public List<Transaction> GetByUserId(int userId);
    public Transaction Add(string description, double amount, DateOnly date, int userId, int categoryId);
    public bool Edit(int id, double amount, string description, DateOnly date, int userId, int categoryId);
    public bool Delete(int id);
}