using System.Security.Authentication;
using Application.Exceptions;
using Application.Services;
using TestLayer.MockData;

namespace TestLayer.Tests;

[TestClass]
public class UserTests
{
    [TestMethod]
    [DataRow("", "johndoe@gmail.com", "password", "2000-01-15")]
    [DataRow("   ", "jane@example.com", "secure123", "1995-07-20")]
    public void Register_NameIsEmptyException(string name, string email, string password, string dateOfBirthString)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());
        DateOnly dateOfBirth;
        try
        {
            dateOfBirth = DateOnly.Parse(dateOfBirthString);
        }
        catch (FormatException ex)
        {
            Assert.Fail($"Kon datumstring '{dateOfBirthString}' niet parsen: {ex.Message}");
            return;
        }

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Register(name, email, password, dateOfBirth)
        );

        //Assert
        Assert.AreEqual($"Name cannot be empty: {name}", exception.Message);
    }

    [TestMethod]
    [DataRow("John Doe", "", "password", "2000-01-15")]
    [DataRow("Jane Doe", "   ", "secure123", "1995-07-20")]
    public void Register_EmailIsEmptyException(string name, string email, string password, string dateOfBirthString)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());
        DateOnly dateOfBirth;
        try
        {
            dateOfBirth = DateOnly.Parse(dateOfBirthString);
        }
        catch (FormatException ex)
        {
            Assert.Fail($"Kon datumstring '{dateOfBirthString}' niet parsen: {ex.Message}");
            return;
        }

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Register(name, email, password, dateOfBirth)
        );

        //Assert
        Assert.AreEqual($"Email cannot be empty: {email}", exception.Message);
    }

    [TestMethod]
    [DataRow("John Doe", "johndoe@gmail.com", "", "2000-01-15")]
    [DataRow("Jane Doe", "janedoe@gmail.com", "   ", "1995-07-20")]
    public void Register_PasswordIsEmptyException(string name, string email, string password, string dateOfBirthString)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());
        DateOnly dateOfBirth;
        try
        {
            dateOfBirth = DateOnly.Parse(dateOfBirthString);
        }
        catch (FormatException ex)
        {
            Assert.Fail($"Kon datumstring '{dateOfBirthString}' niet parsen: {ex.Message}");
            return;
        }

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Register(name, email, password, dateOfBirth)
        );

        //Assert
        Assert.AreEqual($"Password cannot be empty: {password}", exception.Message);
    }

    [TestMethod]
    [DataRow("John Doe", "johndoe#gmail.com", "password", "2000-01-15")]
    [DataRow("Jane Doe", "janedoe.gmail.com", "secure123", "1995-07-20")]
    public void Register_EmailIsInvalidException(string name, string email, string password, string dateOfBirthString)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());
        DateOnly dateOfBirth;
        try
        {
            dateOfBirth = DateOnly.Parse(dateOfBirthString);
        }
        catch (FormatException ex)
        {
            Assert.Fail($"Could not parse dateOfBirthString: '{dateOfBirthString}': {ex.Message}");
            return;
        }

        //Act
        var exception = Assert.ThrowsException<InvalidEmailFormatException>(() =>
            userService.Register(name, email, password, dateOfBirth)
        );

        //Assert
        Assert.AreEqual($"Invalid email format {email}", exception.Message);
    }

    [TestMethod]
    [DataRow("John Doe", "johndoe@gmail.com", "password", "2000-01-15")]
    [DataRow("Jane Doe", "janedoe@gmail.com", "secure123", "1995-07-20")]
    public void Register_EmailAlreadyExistException(string name, string email, string password,
        string dateOfBirthString)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());
        DateOnly dateOfBirth;
        try
        {
            dateOfBirth = DateOnly.Parse(dateOfBirthString);
        }
        catch (FormatException ex)
        {
            Assert.Fail($"Could not parse dateOfBirthString: '{dateOfBirthString}': {ex.Message}");
            return;
        }

        //Act
        var exception = Assert.ThrowsException<UserAlreadyExistsException>(() =>
            userService.Register(name, email, password, dateOfBirth)
        );

        //Assert
        Assert.AreEqual($"User with email {email} already exists", exception.Message);
    }

    [TestMethod]
    [DataRow("Jane Doe", "janedoe22@gmail.com", "secure123", "1995-07-20")]
    public void Register_ValidCredentials_ReturnsUser(string name, string email, string password,
        string dateOfBirthString)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());
        DateOnly dateOfBirth;
        try
        {
            dateOfBirth = DateOnly.Parse(dateOfBirthString);
        }
        catch (FormatException ex)
        {
            Assert.Fail($"Could not parse dateOfBirthString: '{dateOfBirthString}': {ex.Message}");
            return;
        }

        // Act
        var user = userService.Register(name, email, password, dateOfBirth);

        // Assert
        Assert.IsNotNull(user, "User should not be null");
        Assert.IsNotNull(user, "User should not be null");
        Assert.AreEqual(name, user.Name, "Name should match");
        Assert.AreEqual(email, user.Email, "Email should match");
        Assert.AreEqual(dateOfBirth, user.DateOfBirth, "DateOfBirth should match");
    }

    [TestMethod]
    [DataRow("", "password", "supersecret_key_12345_12345_12345", "issuer")]
    [DataRow("   ", "password", "supersecret_key_12345_12345_12345", "issuer")]
    public void Authenticate_EmailIsEmptyException(string email, string password, string jwtKey, string jwtIssuer)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Authenticate(email, password, jwtKey, jwtIssuer)
        );

        //Assert
        Assert.AreEqual($"Email cannot be empty: {email}", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe@gmail.com", "", "supersecret_key_12345_12345_12345", "issuer")]
    [DataRow("johndoe@gmail.com", "   ", "supersecret_key_12345_12345_12345", "issuer")]
    public void Authenticate_PasswordIsEmptyException(string email, string password, string jwtKey, string jwtIssuer)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Authenticate(email, password, jwtKey, jwtIssuer)
        );

        //Assert
        Assert.AreEqual($"Password cannot be empty: {password}", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe@gmail.com", "password", "", "issuer")]
    [DataRow("johndoe@gmail.com", "password", "   ", "issuer")]
    public void Authenticate_JwtKeyIsEmptyException(string email, string password, string jwtKey, string jwtIssuer)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Authenticate(email, password, jwtKey, jwtIssuer)
        );

        //Assert
        Assert.AreEqual($"JWT key cannot be empty: {jwtKey}", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe@gmail.com", "password", "supersecret_key_12345_12345_12345", "")]
    [DataRow("johndoe@gmail.com", "password", "supersecret_key_12345_12345_12345", "   ")]
    public void Authenticate_JwtIssuerIsEmptyException(string email, string password, string jwtKey, string jwtIssuer)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Authenticate(email, password, jwtKey, jwtIssuer)
        );

        //Assert
        Assert.AreEqual($"JWT issuer cannot be empty: {jwtIssuer}", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe22@gmail.com", "password", "supersecret_key_12345_12345_12345", "issuer")]
    [DataRow("janedoe18@gmail.com", "password", "supersecret_key_12345_12345_12345", "issuer")]
    public void Authenticate_UserNotFoundException(string email, string password, string jwtKey, string jwtIssuer)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        //Act
        var exception = Assert.ThrowsException<UserNotFoundException>(() =>
            userService.Authenticate(email, password, jwtKey, jwtIssuer)
        );

        //Assert
        Assert.AreEqual($"User not found", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe@gmail.com", "secret", "supersecret_key_12345_12345_12345", "issuer")]
    [DataRow("janedoe@gmail.com", "password", "supersecret_key_12345_12345_12345", "issuer")]
    public void Authenticate_InvalidPasswordException(string email, string password, string jwtKey, string jwtIssuer)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        //Act
        var exception = Assert.ThrowsException<AuthenticationException>(() =>
            userService.Authenticate(email, password, jwtKey, jwtIssuer)
        );

        //Assert
        Assert.AreEqual($"Invalid email or password", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe@gmail.com", "password", "supersecret_key_12345_12345_12345", "myIssuer")]
    public void Authenticate_ValidCredentials_ReturnsAuthenticationDto(
        string email, string password, string jwtKey, string jwtIssuer)
    {
        // Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        // Act
        var dto = userService.Authenticate(email, password, jwtKey, jwtIssuer);

        // Assert
        Assert.IsNotNull(dto, "AuthenticationDto should not be null");
        Assert.IsFalse(string.IsNullOrWhiteSpace(dto.Token), "Token should not be empty");
        Assert.IsNotNull(dto.User, "User should not be null");
        Assert.AreEqual(email, dto.User.Email, "Email should match");
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetById_IdZeroOrNegativeException(int userId)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.GetById(userId)
        );

        //Assert
        Assert.AreEqual($"User ID must be greater than zero: {userId}", exception.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void GetById_UserNotFoundException(int userId)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        //Act
        var exception = Assert.ThrowsException<UserNotFoundException>(() =>
            userService.GetById(userId)
        );

        //Assert
        Assert.AreEqual($"User with ID {userId} not found", exception.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetById_ReturnsUser(int userId)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        //Act
        var user = userService.GetById(userId);


        //Assert
        Assert.IsNotNull(user, "User should not be null");
        Assert.IsNotNull(user, "User should not be null");
        Assert.AreEqual(userId, user.Id, "User id should match");
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void GetByEmail_EmailIsEmptyException(string email)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.GetByEmail(email)
        );

        //Assert
        Assert.AreEqual($"Email cannot be empty: {email}", exception.Message);
    }

    [TestMethod]
    [DataRow("frankdoe@gmail.com")]
    [DataRow("hansdoe@gmail.com")]
    public void GetByEmail_UserNotFoundException(string email)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        //Act
        var exception = Assert.ThrowsException<UserNotFoundException>(() =>
            userService.GetByEmail(email)
        );

        //Assert
        Assert.AreEqual($"User with email {email} not found", exception.Message);
    }

    [TestMethod]
    [DataRow("johndoe@gmail.com")]
    [DataRow("janedoe@gmail.com")]
    public void GetByEmail_ReturnsUser(string email)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        //Act
        var user = userService.GetByEmail(email);


        //Assert
        Assert.IsNotNull(user, "User should not be null");
        Assert.IsNotNull(user, "User should not be null");
        Assert.AreEqual(email, user.Email, "User id should match");
    }

    [TestMethod]
    public void Update_UserIsNullException()
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());
        User user = null!;

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Update(user)
        );

        //Assert
        Assert.AreEqual($"User cannot be null: {user}", exception.Message);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Update_IdZeroOrNegativeException(int userId)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());
        User user = new User(userId, "Test User", "test@example.com", "hashedpassword", new DateOnly(1990, 1, 1));

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            userService.Update(user)
        );

        //Assert
        Assert.AreEqual($"User ID must be greater than zero: {user.Id}", exception.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void Update_UserNotFoundException(int userId)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());
        User user = new User(userId, "Test User", "test@example.com", "hashedpassword", new DateOnly(1990, 1, 1));

        //Act
        var exception = Assert.ThrowsException<UserNotFoundException>(() =>
            userService.Update(user)
        );

        //Assert
        Assert.AreEqual($"User with ID {user.Id} not found", exception.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void Update_ValidUser_ReturnsTrue(int userId)
    {
        //Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());
        User existingUser = userService.GetById(userId);
        User updatedUser = new User(existingUser.Id, "Updated Name", existingUser.Email, existingUser.PasswordHash,
            existingUser.DateOfBirth);

        //Act
        bool result = userService.Update(updatedUser);

        //Assert
        Assert.IsTrue(result, "Update should return true for valid user");
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Delete_IdZeroOrNegative_ReturnsFalse(int userId)
    {
        // Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        // Act
        var result = userService.Delete(userId);

        // Assert
        Assert.IsFalse(result, "Delete should return false for zero or negative userId");
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void Delete_UserNotFound_ReturnsFalse(int userId)
    {
        // Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        // Act
        var result = userService.Delete(userId);

        // Assert
        Assert.IsFalse(result, "Delete should return false if user is not found");
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void Delete_ValidUser_ReturnsTrue(int userId)
    {
        // Arrange
        var userService = new UserService(new UserRepositoryMock(), new DummyLogger<UserService>());

        // Act
        var result = userService.Delete(userId);

        // Assert
        Assert.IsTrue(result, "Delete should return true for valid user");
    }
}