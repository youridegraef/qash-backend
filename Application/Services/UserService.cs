using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Mail;
using System.Security.Authentication;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public User Register(string name, string email, string password, DateOnly dateOfBirth)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            // Validate email format
            MailAddress m = new MailAddress(email);

            // Check if user already exists
            if (_userRepository.FindByEmail(email) != null)
                throw new UserAlreadyExistsException($"User with email {email} already exists");

            string hashedPassword = PasswordHasher.HashPassword(password);
            User newUser = new User(name, email, hashedPassword, dateOfBirth);
            _userRepository.Add(newUser);
            return newUser;
        }
        catch (FormatException ex)
        {
            // Log the error
            throw new InvalidEmailFormatException($"Invalid email format {ex}");
        }
        catch (UserAlreadyExistsException)
        {
            // Re-throw the exception
            throw;
        }
        catch (ArgumentException)
        {
            // Re-throw the exception
            throw;
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Error registering user: {ex.Message}, {ex.StackTrace}");
            throw new RegistrationFailedException("Error registering user", ex);
        }
    }

    public AuthenticationDto Authenticate(string email, string password, string jwtKey, string jwtIssuer)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new ArgumentException("JWT key cannot be empty", nameof(jwtKey));

            if (string.IsNullOrWhiteSpace(jwtIssuer))
                throw new ArgumentException("JWT issuer cannot be empty", nameof(jwtIssuer));

            User? user = _userRepository.FindByEmail(email);

            if (user == null)
                throw new UserNotFoundException("User not found");

            if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
                throw new AuthenticationException("Invalid email or password");

            // JWT claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim("name", user.Name)
            };

            // JWT settings
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            AuthenticationDto dto = new AuthenticationDto(tokenString, user);
            return dto;
        }
        catch (UserNotFoundException)
        {
            // Re-throw the exception
            throw;
        }
        catch (AuthenticationException)
        {
            // Re-throw the exception
            throw;
        }
        catch (ArgumentException)
        {
            // Re-throw the exception
            throw;
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Authentication error: {ex.Message}, {ex.StackTrace}");
            throw new AuthenticationFailedException("Authentication failed", ex);
        }
    }

    public User GetById(int userId)
    {
        try
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be greater than zero", nameof(userId));

            User? user = _userRepository.FindById(userId);

            if (user == null)
                throw new UserNotFoundException($"User with ID {userId} not found");

            return user;
        }
        catch (UserNotFoundException)
        {
            // Re-throw the exception
            throw;
        }
        catch (ArgumentException)
        {
            // Re-throw the exception
            throw;
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Error retrieving user by ID: {ex.Message}, {ex.StackTrace}");
            throw new UserRetrievalException($"Error retrieving user with ID {userId}", ex);
        }
    }

    public User GetByEmail(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            User? user = _userRepository.FindByEmail(email);

            if (user == null)
                throw new UserNotFoundException($"User with email {email} not found");

            return user;
        }
        catch (UserNotFoundException)
        {
            // Re-throw the exception
            throw;
        }
        catch (ArgumentException)
        {
            // Re-throw the exception
            throw;
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Error retrieving user by email: {ex.Message}, {ex.StackTrace}");
            throw new UserRetrievalException($"Error retrieving user with email {email}", ex);
        }
    }

    public bool Update(User user)
    {
        try
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null");

            if (user.Id <= 0)
                throw new ArgumentException("User ID must be greater than zero", nameof(user));

            // Check if user exists
            if (_userRepository.FindById(user.Id) == null)
                throw new UserNotFoundException($"User with ID {user.Id} not found");

            _userRepository.Edit(user);
            return true;
        }
        catch (UserNotFoundException ex)
        {
            // Log the error
            Console.WriteLine($"User not found during update: {ex.Message}");
            return false;
        }
        catch (ArgumentException ex)
        {
            // Log the error
            Console.WriteLine($"Invalid argument during user update: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Error updating user: {ex.Message}, {ex.StackTrace}");
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("User ID must be greater than zero", nameof(id));

            User? user = _userRepository.FindById(id);

            if (user == null)
                throw new UserNotFoundException($"User with ID {id} not found");

            _userRepository.Delete(user);
            return true;
        }
        catch (UserNotFoundException ex)
        {
            // Log the error
            Console.WriteLine($"User not found during delete: {ex.Message}");
            return false;
        }
        catch (ArgumentException ex)
        {
            // Log the error
            Console.WriteLine($"Invalid argument during user delete: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Error deleting user: {ex.Message}, {ex.StackTrace}");
            return false;
        }
    }
}