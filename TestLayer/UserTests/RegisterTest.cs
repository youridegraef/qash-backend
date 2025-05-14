using System.Security.Authentication;
using Application.Exceptions;
using Application.Interfaces;
using Application.Services;
using TestLayer.MockData;

namespace TestLayer.UserTests;

[TestClass]
public class RegisterTest
{
    [TestMethod]
    [DataRow("", "johndoe@gmail.com", "password", "2000-01-15")]
    [DataRow("   ", "jane@example.com", "secure123", "1995-07-20")]
    public void Register_NameIsEmptyException(string name, string email, string password, string dateOfBirthString)
    {
        //Arrange
        IUserService userService = new UserService(new UserRepositoryMock());
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
        IUserService userService = new UserService(new UserRepositoryMock());
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
        IUserService userService = new UserService(new UserRepositoryMock());
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
        IUserService userService = new UserService(new UserRepositoryMock());
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
        IUserService userService = new UserService(new UserRepositoryMock());
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
        IUserService userService = new UserService(new UserRepositoryMock());
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
}