using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Security.Authentication;
using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Services;

public class UserService(IUserRepository _userRepository) : IUserService
{
    public User Register(string name, string email, string password, DateOnly dateOfBirth)
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

    public User Authenticate(string email, string password)
    {
        User? user = GetByEmail(email);

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

    public User GetById(int userId)
    {
        User? user = _userRepository.FindById(userId);

        if (user != null)
        {
            return user;
        }

        throw new UserNotFoundException("User not found");
    }

    public User GetByEmail(string email)
    {
        User? user = _userRepository.FindByEmail(email);

        if (user != null)
        {
            return user;
        }

        throw new UserNotFoundException("User not found");
    }

    public void Update(User user)
    {
        _userRepository.Edit(user);
    }

    public void Delete(int id)
    {
        try
        {
            User user = _userRepository.FindById(id);
            _userRepository.Delete(user);
        }
        catch
        {
            throw new UserNotFoundException($"User with id: {id} not found.");
        }
    }
}