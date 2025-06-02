using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface IBudgetRepository {
    public Budget FindById(int id);
    public Budget FindByCategoryId(int categoryId);

    public Budget Add(Budget budget);

    public bool Edit(Budget budget);

    public bool Delete(Budget budget);
}