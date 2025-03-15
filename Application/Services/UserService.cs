using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Security.Authentication;
using Application.Interfaces;
using Application.Domain;
using Application.Exceptions;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User RegisterUser(string name, string email, string password, DateOnly dateOfBirth)
    {
        string hashedPassword = PasswordHasher.HashPassword(password);

        try
        {
            MailAddress m = new MailAddress(email);

            User newUser = new User(name, email, hashedPassword, dateOfBirth);
            _userRepository.Add(newUser);
            return newUser;
        }
        catch (FormatException)
        {
            throw new InvalidEmailFormatException("Invalid email format");
        }
    }

    public User AuthenticateUser(string email, string password)
    {
        User? user = GetUserByEmail(email);

        if (user == null)
        {
            throw new AuthenticationException("Invalid email or password");
        }

        if (PasswordHasher.VerifyPassword(password, user.PasswordHash))
        {
            return user;
        }

        throw new AuthenticationException("Invalid email or password");
    }

    public User GetUserById(int userId)
    {
        User? user = _userRepository.FindById(userId);

        if (user != null)
        {
            return user;
        }

        throw new UserNotFoundException("User not found");
    }

    public User GetUserByEmail(string email)
    {
        User? user = _userRepository.FindByEmail(email);

        if (user != null)
        {
            return user;
        }

        throw new UserNotFoundException("User not found");
    }

    public void UpdateUser(User user)
    {
        _userRepository.Edit(user);
    }

    public void DeleteUser(User user)
    {
        _userRepository.Delete(user);
    }
}