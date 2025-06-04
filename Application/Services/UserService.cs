using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Mail;
using System.Security.Authentication;
using Application.Domain;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class UserService(IUserRepository userRepository, ILogger<UserService> logger)
    : IUserService {
    public User Register(string name, string email, string password) {
        try {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"Name cannot be empty: {name}");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException($"Email cannot be empty: {email}");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException($"Password cannot be empty: {password}");

            _ = new MailAddress(email); //Check Email Format > FormatException

            if (userRepository.IsEmailAvailable(email) == false)
                throw new UserAlreadyExistsException($"User with email {email} already exists");

            string hashedPassword = PasswordHasher.HashPassword(password);
            User newUser = new User(name, email, hashedPassword);
            userRepository.Add(newUser);
            return newUser;
        }
        catch (ArgumentException ex) {
            logger.LogError(ex, $"Argument error during registration: {ex.Message}");
            throw;
        }
        catch (UserAlreadyExistsException ex) {
            logger.LogError(ex, $"User already exists: {email}");
            throw;
        }
        catch (FormatException ex) {
            logger.LogError(ex, $"Invalid email format: {email}");
            throw new InvalidEmailFormatException($"Invalid email format {email}");
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, $"Database error during registration for {email}");
            throw new DatabaseException("Database error during registration", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Error registering user: {email}");
            throw new RegistrationFailedException("Error registering user", ex);
        }
    }

    public AuthenticationDto Authenticate(string email, string password, string jwtKey, string jwtIssuer) {
        try {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException($"Email cannot be empty: {email}");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException($"Password cannot be empty: {password}");

            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new ArgumentException($"JWT key cannot be empty: {jwtKey}");

            if (string.IsNullOrWhiteSpace(jwtIssuer))
                throw new ArgumentException($"JWT issuer cannot be empty: {jwtIssuer}");

            User? user = userRepository.FindByEmail(email);

            if (user == null)
                throw new UserNotFoundException("User not found");

            if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
                throw new AuthenticationException("Invalid email or password");

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim("name", user.Name)
            };

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
            AuthenticationDto dto = new AuthenticationDto(user.Id, user.Name, user.Email, tokenString);
            return dto;
        }
        catch (UserNotFoundException ex) {
            logger.LogError(ex, $"User not found during authentication: {email}");
            throw;
        }
        catch (AuthenticationException ex) {
            logger.LogError(ex, $"Authentication failed for user: {email}");
            throw;
        }
        catch (ArgumentException ex) {
            logger.LogError(ex, $"Argument error during authentication: {ex.Message}");
            throw;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, $"Database error during registration for {email}");
            throw new DatabaseException("Database error during registration", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Authentication failed for user: {email}");
            throw new AuthenticationFailedException("Authentication failed", ex);
        }
    }

    public User GetById(int userId) {
        try {
            if (userId <= 0)
                throw new ArgumentException($"User ID must be greater than zero: {userId}");

            User? user = userRepository.FindById(userId);

            if (user == null)
                throw new UserNotFoundException($"User with ID {userId} not found");

            return user;
        }
        catch (UserNotFoundException ex) {
            logger.LogError(ex, $"User with ID {userId} not found.");
            throw;
        }
        catch (ArgumentException ex) {
            logger.LogError(ex, $"Argument error in GetById: {ex.Message}");
            throw;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, $"Database error during registration for {userId}");
            throw new DatabaseException("Database error during registration", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Error retrieving user with ID {userId}");
            throw new UserRetrievalException($"Error retrieving user with ID {userId}", ex);
        }
    }

    public User GetByEmail(string email) {
        try {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException($"Email cannot be empty: {email}");

            User? user = userRepository.FindByEmail(email);

            if (user == null)
                throw new UserNotFoundException($"User with email {email} not found");

            return user;
        }
        catch (UserNotFoundException ex) {
            logger.LogError(ex, $"User with email {email} not found.");
            throw;
        }
        catch (ArgumentException ex) {
            logger.LogError(ex, $"Argument error in GetByEmail: {ex.Message}");
            throw;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, $"Database error during registration for {email}");
            throw new DatabaseException("Database error during registration", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Error retrieving user with email {email}");
            throw new UserRetrievalException($"Error retrieving user with email {email}", ex);
        }
    }

    public bool Update(User user) {
        try {
            if (user == null)
                throw new ArgumentException($"User cannot be null: {user}");

            if (user.Id <= 0)
                throw new ArgumentException($"User ID must be greater than zero: {user.Id}");

            if (userRepository.FindById(user.Id) == null!)
                throw new UserNotFoundException($"User with ID {user.Id} not found");

            userRepository.Edit(user);
            return true;
        }
        catch (UserNotFoundException ex) {
            logger.LogError(ex, $"User with ID {user.Id} not found for update.");
            throw;
        }
        catch (ArgumentException ex) {
            logger.LogError(ex, $"Argument error in Update: {ex.Message}");
            throw;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, $"Database error during registration");
            throw new DatabaseException("Database error during registration", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Error updating user with ID {user.Id}");
            throw;
        }
    }

    public bool Delete(int id) {
        try {
            if (id <= 0)
                throw new ArgumentException($"User ID must be greater than zero: {id}");

            User? user = userRepository.FindById(id);

            if (user == null)
                throw new UserNotFoundException($"User with ID {id} not found");

            userRepository.Delete(user);
            return true;
        }
        catch (UserNotFoundException ex) {
            logger.LogError(ex, $"User with ID {id} not found for deletion.");
            return false;
        }
        catch (ArgumentException ex) {
            logger.LogError(ex, $"Argument error in Delete: {ex.Message}");
            return false;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, $"Database error during registration for {id}");
            throw new DatabaseException("Database error during registration", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Error deleting user with ID {id}");
            return false;
        }
    }
}