using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITransactionService
{
    public Transaction GetById(int id);
    public List<Transaction> GetAll();
    public List<Transaction> GetAllByUserId(int userId);
    public Transaction Add(double amount, DateOnly date, int userId, int categoryId);
    public bool Edit(Transaction transaction);
    public bool Delete(int id);
    public bool AssignTag(int transactionId, int tagId);
    public bool UnAssignTag(int transactionId, int tagId);
}