using System.Security.Authentication;
using Application.Exceptions;
using Application.Services;
using Application.Interfaces;
using Moq;

namespace TestLayer;

[TestClass]
public class UserTests
{
    [TestMethod]
    [DataRow("", "johndoe@gmail.com", "password")]
    [DataRow("   ", "jane@example.com", "secure123")]
    public void Register_NameIsEmptyException(string name, string email, string password)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Register(name, email, password)
        );
        Assert.AreEqual($"Name cannot be empty: {name}", exception.Message);
    }

    [TestMethod]
    [DataRow("John Doe", "", "password")]
    [DataRow("Jane Doe", "   ", "secure123")]
    public void Register_EmailIsEmptyException(string name, string email, string password)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);


        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Register(name, email, password)
        );
        Assert.AreEqual($"Email cannot be empty: {email}", exception.Message);
    }

    [TestMethod]
    [DataRow("John Doe", "johndoe@gmail.com", "")]
    [DataRow("Jane Doe", "janedoe@gmail.com", "   ")]
    public void Register_PasswordIsEmptyException(string name, string email, string password)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);


        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Register(name, email, password)
        );
        Assert.AreEqual($"Password cannot be empty: {password}", exception.Message);
    }

    [TestMethod]
    [DataRow("John Doe", "johndoe#gmail.com", "password")]
    [DataRow("Jane Doe", "janedoe.gmail.com", "secure123")]
    public void Register_EmailIsInvalidException(string name, string email, string password)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);


        var exception = Assert.ThrowsException<InvalidEmailFormatException>(() =>
            userService.Register(name, email, password)
        );
        Assert.AreEqual($"Invalid email format {email}", exception.Message);
    }

    [TestMethod]
    [DataRow("John Doe", "johndoe@gmail.com", "password")]
    [DataRow("Jane Doe", "janedoe@gmail.com", "secure123")]
    public void Register_EmailAlreadyExistException(string name, string email, string password)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.FindByEmail(email))
            .Returns(new User(1, name, email, "hash"));
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);


        var exception = Assert.ThrowsException<UserAlreadyExistsException>(() =>
            userService.Register(name, email, password)
        );
        Assert.AreEqual($"User with email {email} already exists", exception.Message);
    }

    [TestMethod]
    [DataRow("Jane Doe", "janedoe22@gmail.com", "secure123")]
    public void Register_ValidCredentials_ReturnsUser(string name, string email, string password)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.IsEmailAvailable(email)).Returns(true);
        userRepoMock.Setup(r => r.Add(It.IsAny<User>())).Returns(3);
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);


        var user = userService.Register(name, email, password);

        Assert.IsNotNull(user, "User should not be null");
        Assert.AreEqual(name, user.Name, "Name should match");
        Assert.AreEqual(email, user.Email, "Email should match");
    }

    [TestMethod]
    public void Register_DatabaseException_IsWrapped()
    {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.IsEmailAvailable(It.IsAny<string>())).Returns(true);
        userRepoMock.Setup(r => r.Add(It.IsAny<User>())).Throws(new DatabaseException());
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        // Act
        var ex = Assert.ThrowsException<DatabaseException>(() =>
            userService.Register("Test", "test@example.com", "password")
        );
        // Assert
        Assert.AreEqual("Database error during registration", ex.Message);
    }

    [TestMethod]
    [DataRow("", "password", "supersecret_key_12345_12345_12345", "issuer")]
    [DataRow("   ", "password", "supersecret_key_12345_12345_12345", "issuer")]
    public void Authenticate_EmailIsEmptyException(string email, string password, string jwtKey, string jwtIssuer)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Authenticate(email, password, jwtKey, jwtIssuer)
        );
        Assert.AreEqual($"Email cannot be empty: {email}", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe@gmail.com", "", "supersecret_key_12345_12345_12345", "issuer")]
    [DataRow("johndoe@gmail.com", "   ", "supersecret_key_12345_12345_12345", "issuer")]
    public void Authenticate_PasswordIsEmptyException(string email, string password, string jwtKey, string jwtIssuer)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Authenticate(email, password, jwtKey, jwtIssuer)
        );
        Assert.AreEqual($"Password cannot be empty: {password}", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe@gmail.com", "password", "", "issuer")]
    [DataRow("johndoe@gmail.com", "password", "   ", "issuer")]
    public void Authenticate_JwtKeyIsEmptyException(string email, string password, string jwtKey, string jwtIssuer)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Authenticate(email, password, jwtKey, jwtIssuer)
        );
        Assert.AreEqual($"JWT key cannot be empty: {jwtKey}", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe@gmail.com", "password", "supersecret_key_12345_12345_12345", "")]
    [DataRow("johndoe@gmail.com", "password", "supersecret_key_12345_12345_12345", "   ")]
    public void Authenticate_JwtIssuerIsEmptyException(string email, string password, string jwtKey, string jwtIssuer)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Authenticate(email, password, jwtKey, jwtIssuer)
        );
        Assert.AreEqual($"JWT issuer cannot be empty: {jwtIssuer}", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe22@gmail.com", "password", "supersecret_key_12345_12345_12345", "issuer")]
    [DataRow("janedoe18@gmail.com", "password", "supersecret_key_12345_12345_12345", "issuer")]
    public void Authenticate_UserNotFoundException(string email, string password, string jwtKey, string jwtIssuer)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.FindByEmail(email)).Returns((User)null!);
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<UserNotFoundException>(() =>
            userService.Authenticate(email, password, jwtKey, jwtIssuer)
        );
        Assert.AreEqual($"User not found", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe@gmail.com", "secret", "supersecret_key_12345_12345_12345", "issuer")]
    [DataRow("janedoe@gmail.com", "password", "supersecret_key_12345_12345_12345", "issuer")]
    public void Authenticate_InvalidPasswordException(string email, string password, string jwtKey, string jwtIssuer)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("not_the_hash");
        userRepoMock.Setup(r => r.FindByEmail(email))
            .Returns(new User(1, "Test", email, passwordHash));
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<AuthenticationException>(() =>
            userService.Authenticate(email, password, jwtKey, jwtIssuer)
        );
        Assert.AreEqual($"Invalid email or password", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe@gmail.com", "password", "supersecret_key_12345_12345_12345", "myIssuer")]
    public void Authenticate_ValidCredentials_ReturnsAuthenticationDto(
        string email, string password, string jwtKey, string jwtIssuer)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var user = new User(1, "John Doe", email, PasswordHasher.HashPassword(password));
        userRepoMock.Setup(r => r.FindByEmail(email)).Returns(user);
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var dto = userService.Authenticate(email, password, jwtKey, jwtIssuer);

        Assert.IsNotNull(dto, "AuthenticationDto should not be null");
        Assert.IsFalse(string.IsNullOrWhiteSpace(dto.Token), "Token should not be empty");
        Assert.IsNotNull(dto.Id, "User id should not be null");
        Assert.AreEqual(email, dto.Email, "Email should match");
    }

    [TestMethod]
    public void Authenticate_DatabaseException_IsWrapped()
    {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.FindByEmail(It.IsAny<string>())).Throws(new DatabaseException("DB error"));
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        // Act
        var ex = Assert.ThrowsException<DatabaseException>(() =>
            userService.Authenticate("test@example.com", "password", "jwtKey", "jwtIssuer")
        );
        // Assert
        Assert.AreEqual("Database error during registration", ex.Message);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetById_IdZeroOrNegativeException(int userId)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.GetById(userId)
        );
        Assert.AreEqual($"User ID must be greater than zero: {userId}", exception.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void GetById_UserNotFoundException(int userId)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.FindById(userId)).Returns((User)null!);
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<UserNotFoundException>(() =>
            userService.GetById(userId)
        );
        Assert.AreEqual($"User with ID {userId} not found", exception.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetById_ReturnsUser(int userId)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var user = new User(userId, "Test", "test@example.com", "hash");
        userRepoMock.Setup(r => r.FindById(userId)).Returns(user);
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var result = userService.GetById(userId);

        Assert.IsNotNull(result, "User should not be null");
        Assert.AreEqual(userId, result.Id, "User id should match");
    }

    [TestMethod]
    public void GetById_DatabaseException_IsWrapped()
    {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.FindById(It.IsAny<int>())).Throws(new DatabaseException("DB error"));
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        // Act
        var ex = Assert.ThrowsException<DatabaseException>(() =>
            userService.GetById(1)
        );
        // Assert
        Assert.AreEqual("Database error during registration", ex.Message);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void GetByEmail_EmailIsEmptyException(string email)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.GetByEmail(email)
        );
        Assert.AreEqual($"Email cannot be empty: {email}", exception.Message);
    }

    [TestMethod]
    [DataRow("frankdoe@gmail.com")]
    [DataRow("hansdoe@gmail.com")]
    public void GetByEmail_UserNotFoundException(string email)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.FindByEmail(email)).Returns((User)null!);
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<UserNotFoundException>(() =>
            userService.GetByEmail(email)
        );
        Assert.AreEqual($"User with email {email} not found", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe@gmail.com")]
    [DataRow("janedoe@gmail.com")]
    public void GetByEmail_ReturnsUser(string email)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var user = new User(1, "Test", email, "hash");
        userRepoMock.Setup(r => r.FindByEmail(email)).Returns(user);
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var result = userService.GetByEmail(email);

        Assert.IsNotNull(result, "User should not be null");
        Assert.AreEqual(email, result.Email, "User id should match");
    }

    [TestMethod]
    public void GetByEmail_DatabaseException_IsWrapped()
    {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.FindByEmail(It.IsAny<string>())).Throws(new DatabaseException("DB error"));
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        // Act
        var ex = Assert.ThrowsException<DatabaseException>(() =>
            userService.GetByEmail("test@example.com")
        );
        // Assert
        Assert.AreEqual("Database error during registration", ex.Message);
    }

    [TestMethod]
    public void Update_UserIsNullException()
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);
        User user = null!;

        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Update(user)
        );
        Assert.AreEqual($"User cannot be null: {user}", exception.Message);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Update_IdZeroOrNegativeException(int userId)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var user = new User(userId, "Test User", "test@example.com", "hashedpassword");
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Update(user)
        );
        Assert.AreEqual($"User ID must be greater than zero: {user.Id}", exception.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void Update_UserNotFoundException(int userId)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.FindById(userId)).Returns((User)null!);
        var user = new User(userId, "Test User", "test@example.com", "hashedpassword");
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<UserNotFoundException>(() =>
            userService.Update(user)
        );
        Assert.AreEqual($"User with ID {user.Id} not found", exception.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void Update_ValidUser_ReturnsTrue(int userId)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var user = new User(userId, "Test User", "test@example.com", "hashedpassword");
        userRepoMock.Setup(r => r.FindById(userId)).Returns(user);
        userRepoMock.Setup(r => r.Edit(user)).Returns(true);
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        bool result = userService.Update(user);

        Assert.IsTrue(result, "Update should return true for valid user");
    }

    [TestMethod]
    public void Update_DatabaseException_IsWrapped()
    {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var user = new User(1, "Test", "test@example.com", "hash");
        userRepoMock.Setup(r => r.FindById(It.IsAny<int>())).Returns(user);
        userRepoMock.Setup(r => r.Edit(It.IsAny<User>())).Throws(new DatabaseException("DB error"));
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        // Act
        var ex = Assert.ThrowsException<DatabaseException>(() =>
            userService.Update(user)
        );
        // Assert
        Assert.AreEqual("Database error during registration", ex.Message);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Delete_IdZeroOrNegative_ReturnsFalse(int userId)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var result = userService.Delete(userId);

        Assert.IsFalse(result, "Delete should return false for zero or negative userId");
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void Delete_UserNotFound_ReturnsFalse(int userId)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.FindById(userId)).Returns((User)null!);
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var result = userService.Delete(userId);

        Assert.IsFalse(result, "Delete should return false if user is not found");
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void Delete_ValidUser_ReturnsTrue(int userId)
    {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var user = new User(userId, "Test User", "test@example.com", "hashedpassword");
        userRepoMock.Setup(r => r.FindById(userId)).Returns(user);
        userRepoMock.Setup(r => r.Delete(user)).Returns(true);
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        var result = userService.Delete(userId);

        Assert.IsTrue(result, "Delete should return true for valid user");
    }

    [TestMethod]
    public void Delete_DatabaseException_IsWrapped()
    {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();
        var userRepoMock = new Mock<IUserRepository>();
        var user = new User(1, "Test", "test@example.com", "hash");
        userRepoMock.Setup(r => r.FindById(It.IsAny<int>())).Returns(user);
        userRepoMock.Setup(r => r.Delete(It.IsAny<User>())).Throws(new DatabaseException("DB error"));
        var userService = new UserService(userRepoMock.Object, loggerMock.Object);

        // Act
        var ex = Assert.ThrowsException<DatabaseException>(() =>
            userService.Delete(1)
        );
        // Assert
        Assert.AreEqual("Database error during registration", ex.Message);
    }
}