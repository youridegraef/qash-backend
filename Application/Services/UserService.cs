using System.Net.Mail;
using System.Security.Authentication;
using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    
    public User Register(string name, string email, string password, DateOnly dateOfBirth)
    {
        try
        {
            MailAddress m = new MailAddress(email);
            string hashedPassword = PasswordHasher.HashPassword(password);
            User newUser = new User(name, email, hashedPassword, dateOfBirth);
            _userRepository.Add(newUser);
            return newUser;
        }
        catch (FormatException)
        {
            throw new InvalidEmailFormatException("Invalid email format");
        }
        catch (Exception)
        {
            throw new Exception("Error registering user");
        }
    }

    public User Authenticate(string email, string password)
    {
        User? user = _userRepository.FindByEmail(email);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
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

    public bool Update(User user)
    {
        try
        {
            _userRepository.Edit(user);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            User user = _userRepository.FindById(id);
            _userRepository.Delete(user);
            return true;
        }
        catch (UserNotFoundException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}