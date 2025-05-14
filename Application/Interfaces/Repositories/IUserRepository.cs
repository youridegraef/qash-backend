using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface IUserRepository
{
    public List<User> FindAll();

    public User? FindById(int id);

    public User? FindByEmail(string email);

    public int Add(User user);

    public bool Edit(User user);

    public bool Delete(User user);
}