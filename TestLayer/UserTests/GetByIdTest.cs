using Application.Exceptions;
using Application.Interfaces;
using Application.Services;
using TestLayer.MockData;

namespace TestLayer.UserTests;

[TestClass]
public class GetByIdTest
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetById_IdZeroOrNegativeException(int userId)
    {
        //Arrange
        IUserService userService = new UserService(new UserRepositoryMock());

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
        IUserService userService = new UserService(new UserRepositoryMock());

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
        IUserService userService = new UserService(new UserRepositoryMock());

        //Act
        var user = userService.GetById(userId);


        //Assert
        Assert.IsNotNull(user, "User should not be null");
        Assert.IsNotNull(user, "User should not be null");
        Assert.AreEqual(userId, user.Id, "User id should match");
    }
}