using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ICategoryRepository
{
    public List<Category> FindAll();

    public Category? FindById(int id);

    public int Add(Category category);

    public bool Edit(Category category);

    public bool Delete(Category category);
}