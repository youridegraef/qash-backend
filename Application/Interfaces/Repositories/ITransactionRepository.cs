using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITransactionRepository
{
    public List<Transaction> FindAll();
    public List<Transaction> FindAllPaged(int page, int pageSize);
    public List<Transaction> FindByUserId(int userId);
    public List<Transaction> FindByUserIdPaged(int userId, int page, int pageSize);

    public Transaction FindById(int id);

    public int Add(Transaction transaction);

    public bool Edit(Transaction transaction);

    public bool Delete(Transaction transaction);
}