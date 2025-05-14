using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Mail;
using System.Security.Authentication;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Services
{
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
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException($"Name cannot be empty: {name}");

                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException($"Email cannot be empty: {email}");

                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException($"Password cannot be empty: {password}");

                MailAddress m = new MailAddress(email);

                if (_userRepository.FindByEmail(email) != null)
                    throw new UserAlreadyExistsException($"User with email {email} already exists");

                string hashedPassword = PasswordHasher.HashPassword(password);
                User newUser = new User(name, email, hashedPassword, dateOfBirth);
                _userRepository.Add(newUser);
                return newUser;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (UserAlreadyExistsException)
            {
                throw;
            }
            catch (FormatException ex)
            {
                throw new InvalidEmailFormatException($"Invalid email format {email}");
            }
            catch (Exception ex)
            {
                throw new RegistrationFailedException("Error registering user", ex);
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

                User? user = _userRepository.FindByEmail(email);

                if (user == null)
                    throw new UserNotFoundException("User not found");

                if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
                    throw new AuthenticationException("Invalid email or password");

                var claims = new[]
                {
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
                AuthenticationDto dto = new AuthenticationDto(tokenString, user);
                return dto;
            }
            catch (UserNotFoundException)
            {
                throw;
            }
            catch (AuthenticationException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AuthenticationFailedException("Authentication failed", ex);
            }
        }

        public User GetById(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException($"User ID must be greater than zero: {userId}");

                User? user = _userRepository.FindById(userId);

                if (user == null)
                    throw new UserNotFoundException($"User with ID {userId} not found");

                return user;
            }
            catch (UserNotFoundException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new UserRetrievalException($"Error retrieving user with ID {userId}", ex);
            }
        }

        public User GetByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException($"Email cannot be empty: {email}");

                User? user = _userRepository.FindByEmail(email);

                if (user == null)
                    throw new UserNotFoundException($"User with email {email} not found");

                return user;
            }
            catch (UserNotFoundException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new UserRetrievalException($"Error retrieving user with email {email}", ex);
            }
        }

        public bool Update(User user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentException($"User cannot be null: {user}");

                if (user.Id <= 0)
                    throw new ArgumentException($"User ID must be greater than zero: {user.Id}");

                if (_userRepository.FindById(user.Id) == null)
                    throw new UserNotFoundException($"User with ID {user.Id} not found");

                _userRepository.Edit(user);
                return true;
            }
            catch (UserNotFoundException ex)
            {
                throw;
            }
            catch (ArgumentException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException($"User ID must be greater than zero: {id}");

                User? user = _userRepository.FindById(id);

                if (user == null)
                    throw new UserNotFoundException($"User with ID {id} not found");

                _userRepository.Delete(user);
                return true;
            }
            catch (UserNotFoundException ex)
            {
                return false;
            }
            catch (ArgumentException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}