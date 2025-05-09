using Application.Domain;
using SQLitePCL;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ICategoryService
{
    public List<Category> GetAll();
    public Category GetById(int id);
    public List<Category> GetByUserId(int userId);
    public List<Category> GetByName(string name);
    public Category Add(string name, int userId);
    public bool Edit(Category category);
    public bool Delete(int id);

    public double CalculateSpendingsByCategory(int categoryId);
    public double CalculateSpendingsByCategoryAndDateRange(int categoryId, DateOnly startDate, DateOnly endDate);
}
