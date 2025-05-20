using Application.Dtos;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface IUserService
{
    User Register(string name, string email, string password);
    AuthenticationDto Authenticate(string email, string password, string jwtKey, string jwtIssuer);
    User GetById(int userId);
    User GetByEmail(string email);
    bool Update(User user);
    bool Delete(int id);
}