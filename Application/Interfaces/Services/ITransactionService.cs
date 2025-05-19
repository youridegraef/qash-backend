using Application.Domain;
using Application.Dtos;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ITransactionService
{
    public Transaction GetById(int id);
    public List<Transaction> GetByUserIdPaged(int userId, int page, int pageSize);
    public List<Transaction> GetByDateRange(DateOnly startDate, DateOnly endDate);
    public List<Transaction> GetByUserId(int userId);
    public Transaction Add(string description, double amount, DateOnly date, int userId, int categoryId);
    public bool Edit(int id, double amount, string description, DateOnly date, int userId, int categoryId);
    public bool Delete(int id);
    public double GetBalance(int userId);
    public double GetExpenses(int userId);
    public double GetIncome(int userId);
    public List<ChartDataDto> GetChartData(int userId);
}