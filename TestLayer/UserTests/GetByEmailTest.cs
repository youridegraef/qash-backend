using Application.Exceptions;
using Application.Interfaces;
using Application.Services;
using TestLayer.MockData;

namespace TestLayer.UserTests;

[TestClass]
public class GetByEmailTest
{
    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void GetByEmail_EmailIsEmptyException(string email)
    {
        //Arrange
        IUserService userService = new UserService(new UserRepositoryMock());

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
        IUserService userService = new UserService(new UserRepositoryMock());

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
        IUserService userService = new UserService(new UserRepositoryMock());

        //Act
        var user = userService.GetByEmail(email);


        //Assert
        Assert.IsNotNull(user, "User should not be null");
        Assert.IsNotNull(user, "User should not be null");
        Assert.AreEqual(email, user.Email, "User id should match");
    }
}