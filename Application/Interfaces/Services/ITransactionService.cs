using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITransactionService
{
    public Transaction AddTransaction(double amount, DateOnly date, int categoryId);
    public Transaction GetTransactionById(int transactionId);
    public List<Transaction> GetAllTransactions(int userId);
    public List<Transaction> GetTransactionsByPeriod(DateOnly start, DateOnly end);
    public List<Transaction> GetTransactionsByCategory(int categoryId);
    public List<Transaction> GetTransactionsByTag(int tagId);
    public void UpdateTransaction(Transaction transaction);
    public void DeleteTransaction(Transaction transaction);
}