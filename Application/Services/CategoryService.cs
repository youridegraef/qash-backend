using Application.Domain;
using Application.Interfaces;

namespace Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;

    public CategoryService(ICategoryRepository categoryRepository, ITransactionRepository transactionRepository)
    {
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
    }

    public List<Category> GetAll()
    {
        try
        {
            return _categoryRepository.FindAll();
        }
        catch
        {
            throw new Exception("No categories found");
        }
    }

    public Category GetById(int id)
    {
        Category category = _categoryRepository.FindById(id);

        if (category != null)
        {
            return category;
        }

        throw new KeyNotFoundException($"No category with id: {id} found.");
    }

    public List<Category> GetByUserId(int userId)
    {
        try
        {
            List<Category> allCategories = _categoryRepository.FindAll();

            var filteredCategories = allCategories
                .Where(c => c.UserId == userId).ToList();

            return filteredCategories;
        }
        catch
        {
            throw new Exception($"No categories with user id: {userId} found.");
        }
    }

    public List<Category> GetByName(string name)
    {
        try
        {
            List<Category> allCategories = _categoryRepository.FindAll();

            var filteredCategories = allCategories
                .Where(c => c.Name == name).ToList();

            return filteredCategories;
        }
        catch
        {
            throw new Exception($"No categories with user id: {name} found.");
        }
    }

    public Category Add(string name, int userId, string colorHexCode)
    {
        try
        {
            Category category = new Category(name, userId, colorHexCode);
            category.Id = _categoryRepository.Add(category);
            return category;
        }
        catch
        {
            throw new Exception($"Category couldn't be added.");
        }
    }

    public bool Edit(Category category)
    {
        try
        {
            return _categoryRepository.Edit(category);
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
            var category = _categoryRepository.FindById(id);
            return _categoryRepository.Delete(category);
        }
        catch
        {
            return false;
        }
    }

    public double CalculateSpendingsByCategory(int categoryId)
    {
        try
        {
            double spendings = 0;
            List<Transaction> transactions = _transactionRepository.FindAll();

            var filteredTransactions = transactions
                .Where(t => t.CategoryId == categoryId).ToList();

            foreach (var transaction in filteredTransactions)
            {
                spendings += transaction.Amount;
            }

            return spendings;
        }
        catch
        {
            throw new Exception($"No transactions found with category id: {categoryId}");
        }
    }
    
    public double CalculateSpendingsByCategoryAndDateRange(int categoryId, DateOnly startDate, DateOnly endDate)
    {
        try
        {
            double spendings = 0;
            List<Transaction> transactions = _transactionRepository.FindAll();

            var filteredTransactions = transactions
                .Where(t => 
                    t.CategoryId == categoryId &&
                    t.Date >= startDate &&
                    t.Date <= endDate
                ).ToList();

            foreach (var transaction in filteredTransactions)
            {
                spendings += transaction.Amount;
            }

            return spendings;
        }
        catch
        {
            throw new Exception($"No transactions found with category id: {categoryId}");
        }
    }
}