// ReSharper disable once CheckNamespace

using Application.Domain;

namespace Application.Interfaces;

public interface ICategoryService
{
    //TODO: Welke methods wil ik hier?

    public List<Category> GetALl();
    public Category GetById(int id);
    public List<Category> GetByUserId(int userId);
    
    public Category Add(string name, int userId);
}