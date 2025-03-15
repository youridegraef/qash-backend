using Application.Domain;

namespace Application.Interfaces;

public interface IBudgetRepository
{
    public List<Budget> FindAll();

    public Budget? FindById(int id);

    public bool Add(Budget budget);

    public bool Edit(Budget budget);

    public bool Delete(Budget budget);
}