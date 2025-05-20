using Application.Domain;
using Application.Dtos;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITransactionRepository
{
    public List<TransactionDto> FindAllPaged(int page, int pageSize);
    public List<TransactionDto> FindByUserId(int userId);
    public List<TransactionDto> FindByUserIdPaged(int userId, int page, int pageSize);

    public TransactionDto FindById(int id);

    public int Add(Transaction transaction);

    public bool Edit(Transaction transaction);

    public bool Delete(Transaction transaction);
}