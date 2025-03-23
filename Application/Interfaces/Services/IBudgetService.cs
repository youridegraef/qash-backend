using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface IBudgetService
{
    public List<Budget> GetAll();
    public Budget GetById(int id);
    public List<Budget> GetByUserId(int userId);
    public List<Budget> GetByName(string name);
    public List<Budget> GetBYNameAndUserId(int userId, string name);
    public Budget Add(DateOnly startDate, DateOnly endDate, double target, int categoryId);
    public Budget Edit(Budget budget);
    public Budget Delete(int id);
}