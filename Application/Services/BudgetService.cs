using Application.Domain;
using Application.Interfaces;

namespace Application.Services;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _budgetRepository;

    public BudgetService(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }
    

    public List<Budget> GetAll()
    {
        try
        {
            return _budgetRepository.FindAll();
        }
        catch
        {
            throw new Exception("No budgets found");
        }
    }

    public Budget GetById(int id)
    {
        Budget budget = _budgetRepository.FindById(id);

        if (budget != null)
        {
            return budget;
        }

        throw new KeyNotFoundException($"No budget with id: {id} found.");
    }

    public List<Budget> GetByCategoryId(int categoryId)
    {

        try
        {
            List<Budget> allBudgets = _budgetRepository.FindAll();

            var filteredBudgets = allBudgets
                .Where(b => b.CategoryId == categoryId).ToList();

            return filteredBudgets;
        }
        catch
        {
            throw new Exception($"No budgets with category id: {categoryId} found.");
        }
    }

    public Budget Add(DateOnly startDate, DateOnly endDate, double target, int categoryId)
    {
        try
        {
            Budget budget = new Budget(startDate, endDate, target, categoryId);
            budget.Id = _budgetRepository.Add(budget);
            return budget;
        }
        catch
        {
            throw new Exception("Budget couldn't be added.");
        }
    }

    public bool Edit(Budget budget)
    {
        try
        {
            return _budgetRepository.Edit(budget);
        }
        catch
        {
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            var budget = _budgetRepository.FindById(id);
            return _budgetRepository.Delete(budget);
        }
        catch
        {
            return false;
        }
    }
}