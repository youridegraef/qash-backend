using Application.Exceptions;
using Application.Interfaces;
using Application.Services;
using TestLayer.MockData;

namespace TestLayer.UserTests;

[TestClass]
public class UpdateTest
{
    [TestMethod]
    public void Update_UserIsNullException()
    {
        //Arrange
        IUserService userService = new UserService(new UserRepositoryMock());
        User user = null;

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
        IUserService userService = new UserService(new UserRepositoryMock());
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
        IUserService userService = new UserService(new UserRepositoryMock());
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
        IUserService userService = new UserService(new UserRepositoryMock());
        User existingUser = userService.GetById(userId);
        User updatedUser = new User(existingUser.Id, "Updated Name", existingUser.Email, existingUser.PasswordHash,
            existingUser.DateOfBirth);

        //Act
        bool result = userService.Update(updatedUser);

        //Assert
        Assert.IsTrue(result, "Update should return true for valid user");
    }
}