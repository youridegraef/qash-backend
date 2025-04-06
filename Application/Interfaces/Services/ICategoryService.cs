using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ICategoryService
{
    public List<Category> GetALl();
    public Category GetById(int id);
    public List<Category> GetByUserId(int userId);
    public List<Category> GetByName(string name);
    public Category Add(string name, int userId);
    public bool Edit(Category category);
    public bool Delete(int id);
}
