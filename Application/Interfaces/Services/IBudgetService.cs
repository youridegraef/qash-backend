using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface IBudgetService {
    public Budget GetById(int id);
    public Budget GetByCategoryId(int categoryId);
    public Budget Add(DateOnly startDate, DateOnly endDate, double target, int categoryId);
    public bool Edit(Budget budget);
    public bool Delete(int id);
}