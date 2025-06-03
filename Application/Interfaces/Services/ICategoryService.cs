using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ICategoryService {
    public Category GetById(int id);
    public List<Category> GetByUserId(int userId);
    public List<Category> GetByUserIdPaged(int userId, int page, int pageSize);
    public Category Add(string name, int userId);
    public bool Edit(Category category);
    public bool Delete(int id);
}