using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ICategoryRepository
{
    public Category FindById(int id);
    public List<Category> FindByUserId(int userId);
    public List<Category> FindByUserIdPaged(int userId, int page, int pageSize);

    public int Add(Category category);

    public bool Edit(Category category);

    public bool Delete(Category category);
}