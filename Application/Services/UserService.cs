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

    public UserAuthenticate Register(string name, string email, string password, DateOnly dateOfBirth)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"Name cannot be empty: {name}");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException($"Email cannot be empty: {email}");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException($"Password cannot be empty: {password}");

            // Validate email format
            MailAddress m = new MailAddress(email);

            // Check if userAuthenticate already exists
            if (_userRepository.FindByEmail(email) != null)
                throw new UserAlreadyExistsException($"UserAuthenticate with email {email} already exists");

            string hashedPassword = PasswordHasher.HashPassword(password);
            UserAuthenticate newUserAuthenticate = new UserAuthenticate(name, email, hashedPassword, dateOfBirth);
            _userRepository.Add(newUserAuthenticate);
            return newUserAuthenticate;
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
            Console.WriteLine($"Error registering userAuthenticate: {ex.Message}, {ex.StackTrace}");
            throw new RegistrationFailedException("Error registering userAuthenticate", ex);
        }
    }

    public AuthenticationDto Authenticate(string email, string password, string jwtKey, string jwtIssuer)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException($"Email cannot be empty: {email}");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException($"Password cannot be empty: {password}");

            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new ArgumentException($"JWT key cannot be empty: {jwtKey}");

            if (string.IsNullOrWhiteSpace(jwtIssuer))
                throw new ArgumentException($"JWT issuer cannot be empty: {jwtIssuer}");

            UserAuthenticate? user = _userRepository.FindByEmail(email);

            if (user == null)
                throw new UserNotFoundException("UserAuthenticate not found");

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

    public UserAuthenticate GetById(int userId)
    {
        try
        {
            if (userId <= 0)
                throw new ArgumentException($"UserAuthenticate ID must be greater than zero: {userId}");

            UserAuthenticate? user = _userRepository.FindById(userId);

            if (user == null)
                throw new UserNotFoundException($"UserAuthenticate with ID {userId} not found");

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
            Console.WriteLine($"Error retrieving userAuthenticate by ID: {ex.Message}, {ex.StackTrace}");
            throw new UserRetrievalException($"Error retrieving userAuthenticate with ID {userId}", ex);
        }
    }

    public UserAuthenticate GetByEmail(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException($"Email cannot be empty: {email}");

            UserAuthenticate? user = _userRepository.FindByEmail(email);

            if (user == null)
                throw new UserNotFoundException($"UserAuthenticate with email {email} not found");

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
            Console.WriteLine($"Error retrieving userAuthenticate by email: {ex.Message}, {ex.StackTrace}");
            throw new UserRetrievalException($"Error retrieving userAuthenticate with email {email}", ex);
        }
    }

    public bool Update(UserAuthenticate userAuthenticate)
    {
        try
        {
            if (userAuthenticate == null)
                throw new ArgumentException($"UserAuthenticate cannot be null: {userAuthenticate}");

            if (userAuthenticate.Id <= 0)
                throw new ArgumentException($"UserAuthenticate ID must be greater than zero: {userAuthenticate.Id}");

            // Check if userAuthenticate exists
            if (_userRepository.FindById(userAuthenticate.Id) == null)
                throw new UserNotFoundException($"UserAuthenticate with ID {userAuthenticate.Id} not found");

            _userRepository.Edit(userAuthenticate);
            return true;
        }
        catch (UserNotFoundException ex)
        {
            // Log the error
            Console.WriteLine($"UserAuthenticate not found during update: {ex.Message}");
            return false;
        }
        catch (ArgumentException ex)
        {
            // Log the error
            Console.WriteLine($"Invalid argument during userAuthenticate update: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Error updating userAuthenticate: {ex.Message}, {ex.StackTrace}");
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException($"UserAuthenticate ID must be greater than zero: {id}");

            UserAuthenticate? user = _userRepository.FindById(id);

            if (user == null)
                throw new UserNotFoundException($"UserAuthenticate with ID {id} not found");

            _userRepository.Delete(user);
            return true;
        }
        catch (UserNotFoundException ex)
        {
            // Log the error
            Console.WriteLine($"UserAuthenticate not found during delete: {ex.Message}");
            return false;
        }
        catch (ArgumentException ex)
        {
            // Log the error
            Console.WriteLine($"Invalid argument during userAuthenticate delete: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Error deleting userAuthenticate: {ex.Message}, {ex.StackTrace}");
            return false;
        }
    }
}