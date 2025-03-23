using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITransactionRepository
{
    public List<Transaction> FindAll();

    public Transaction? FindById(int id);

    public int Add(Transaction transaction);

    public bool Edit(Transaction transaction);

    public bool Delete(Transaction transaction);
    public bool AssignTagToTransaction(int transactionId, int tagId);
    public bool DeleteTagFromTransaction(int transactionId, int tagId);

}