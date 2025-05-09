using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface IUserService
{
    User Register(string name, string email, string password, DateOnly dateOfBirth);
    User Authenticate(string email, string password);
    User GetById(int userId);
    User GetByEmail(string email);
    bool Update(User user);
    bool Delete(int id);
    public double CalculateBalance(int userId);
    public double CalculateExpenses(int userId);
    public double CalculateExpensesByDateRange(int userId, DateOnly startDate, DateOnly endDate);
    public double CalculateIncome(int userId);
    public double CalculateIncomeByDateRange(int userId, DateOnly startDate, DateOnly endDate);
}
