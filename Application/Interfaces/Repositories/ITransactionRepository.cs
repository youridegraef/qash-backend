using Application.Domain;

namespace Application.Interfaces;

public interface ITransactionRepository
{
    public List<Transaction> FindAll();

    public Transaction? FindById(int id);

    public bool Add(Transaction transaction);

    public bool Edit(Transaction transaction);

    public bool Delete(Transaction transaction);

    public List<Tag> FindTransactionTags(int id);
}