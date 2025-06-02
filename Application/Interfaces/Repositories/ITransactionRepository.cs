using Application.Domain;
using Application.Dtos;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITransactionRepository {
    public List<Transaction> FindAllPaged(int page, int pageSize);
    public List<Transaction> FindByUserId(int userId);
    public List<Transaction> FindByUserIdPaged(int userId, int page, int pageSize);
    public Transaction FindById(int id);
    public Transaction Add(Transaction transaction);
    public bool Edit(Transaction transaction);
    public bool Delete(Transaction transaction);
    void AddTagsToTransaction(int transactionId, List<Tag> tags);
}