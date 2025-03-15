using Application.Domain;

namespace Application.Interfaces;

public interface IUserService
{
    User RegisterUser(string name, string email, string password, DateOnly dateOfBirth);
    User AuthenticateUser(string email, string password);
    User GetUserById(int userId);
    User GetUserByEmail(string email);
    void UpdateUser(User user);
    void DeleteUser(User user);
}