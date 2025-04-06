using Application.Domain;
using Application.Interfaces;

namespace Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    
    public List<Category> GetALl()
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

    public Category Add(string name, int userId)
    {
        try
        {
            Category category = new Category(name, userId);
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
}