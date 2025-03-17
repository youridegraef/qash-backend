using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface IUserService
{
    User Register(string name, string email, string password, DateOnly dateOfBirth);
    User Authenticate(string email, string password);
    User GetById(int userId);
    User GetByEmail(string email);
    void Update(User user);
    void Delete(int id);
}