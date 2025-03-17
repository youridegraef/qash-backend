using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITransactionService
{
    public Transaction GetById(int id);
    public List<Transaction> GetAll();
    public List<Transaction> GetAllByUserId(int userId);
    public List<Transaction> GetByPeriod(DateOnly start, DateOnly end);
    public List<Transaction> GetByCategory(int id);
    public List<Transaction> GetByTag(int tagId);
    public Transaction Add(double amount, DateOnly date, int categoryId);
    public bool Edit(Transaction transaction);
    public void Delete(int id);
}