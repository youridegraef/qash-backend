using Application.Domain;
using Application.Dtos;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface IBudgetService {
    public BudgetDto GetById(int id);
    public BudgetDto GetByCategoryId(int categoryId);
    public List<BudgetDto> GetByUserId(int userId);
    public BudgetDto Add(DateOnly startDate, DateOnly endDate, double target, int categoryId);
    public bool Edit(Budget budget);
    public bool Delete(int id);
}