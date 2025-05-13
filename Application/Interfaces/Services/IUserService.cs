using Application.Domain;
using Application.Dtos;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface IUserService
{
    UserAuthenticate Register(string name, string email, string password, DateOnly dateOfBirth);
    AuthenticationDto Authenticate(string email, string password, string jwtKey, string jwtIssuer);
    UserAuthenticate GetById(int userId);
    UserAuthenticate GetByEmail(string email);
    bool Update(UserAuthenticate userAuthenticate);
    bool Delete(int id);
}