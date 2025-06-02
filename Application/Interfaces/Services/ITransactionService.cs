using Application.Domain;
using Application.Dtos;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITransactionService {
    public TransactionDto GetById(int id);
    public List<TransactionDto> GetByUserId(int userId);
    public List<TransactionDto> GetByUserIdPaged(int userId, int page, int pageSize);

    public TransactionDto Add(string description, double amount, DateOnly date, int userId, int categoryId,
        List<Tag> tags);

    public bool Edit(int id, double amount, string description, DateOnly date, int userId, int categoryId,
        List<Tag> tags);

    public bool Delete(int id);
    public double GetBalance(int userId);
    public double GetExpenses(int userId);
    public double GetIncome(int userId);
}