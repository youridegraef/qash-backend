using Application.Domain;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestLayer;

[TestClass]
public class SavingGoalTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetById_IdZeroOrNegative_ThrowsException(int goalId) {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.FindById(goalId))
            .Throws(new ArgumentException($"SavingGoal ID must be greater than zero: {goalId}"));
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetById(goalId));
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
        Assert.AreEqual($"Error retrieving saving goal with id: {goalId}", ex.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void GetById_SavingGoalNotFoundException(int goalId) {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.FindById(goalId)).Returns((SavingGoal)null!);
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<SavingGoalNotFoundException>(() => service.GetById(goalId));
        Assert.AreEqual($"Saving goal with id {goalId} not found.", ex.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetById_ReturnsSavingGoalDto(int goalId) {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        var goal = new SavingGoal(goalId, "Test Goal", 1000, DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1);
        repoMock.Setup(r => r.FindById(goalId)).Returns(goal);
        repoMock.Setup(r => r.FindByUserId(goal.UserId)).Returns(new List<SavingGoal> { goal });
        transactionServiceMock.Setup(t => t.GetBalance(goal.UserId)).Returns(500);
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var result = service.GetById(goalId);

        Assert.IsNotNull(result);
        Assert.AreEqual(goalId, result.Id);
        Assert.AreEqual("Test Goal", result.Name);
        Assert.AreEqual(500, result.AmountSaved);
        Assert.AreEqual(1000, result.Target);
    }

    [TestMethod]
    public void GetById_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.FindById(It.IsAny<int>())).Throws(new DatabaseException("DB error"));
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetById(1));
        Assert.AreEqual("Database error retrieving saving goal with id: 1", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetByUserId_IdZeroOrNegative_ThrowsException(int userId) {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.FindByUserId(userId))
            .Throws(new ArgumentException($"User ID must be greater than zero: {userId}"));
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetByUserId(userId));
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
        Assert.AreEqual($"Error retrieving saving goals for user_id: {userId}", ex.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void GetByUserId_SavingGoalNotFoundException(int userId) {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.FindByUserId(userId)).Returns(new List<SavingGoal>());
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<SavingGoalNotFoundException>(() => service.GetByUserId(userId));
        Assert.AreEqual($"Saving goal with user id {userId} not found.", ex.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetByUserId_ReturnsSavingGoalDtoList(int userId) {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        var goals = new List<SavingGoal> {
            new SavingGoal(1, "Goal 1", 1000, DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), userId),
            new SavingGoal(2, "Goal 2", 2000, DateOnly.FromDateTime(DateTime.Now.AddMonths(2)), userId)
        };
        repoMock.Setup(r => r.FindByUserId(userId)).Returns(goals);
        repoMock.Setup(r => r.FindByUserId(userId)).Returns(goals);
        transactionServiceMock.Setup(t => t.GetBalance(userId)).Returns(1000);
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var result = service.GetByUserId(userId);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Goal 1", result[0].Name);
        Assert.AreEqual("Goal 2", result[1].Name);
    }

    [TestMethod]
    public void GetByUserId_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.FindByUserId(It.IsAny<int>())).Throws(new DatabaseException("DB error"));
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetByUserId(1));
        Assert.AreEqual("Database error retrieving saving goals for user_id: 1", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    [DataRow(1, 1, 10)]
    [DataRow(2, 1, 5)]
    public void GetByUserIdPaged_ReturnsSavingGoalDtoList(int userId, int page, int pageSize) {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        var goals = new List<SavingGoal> {
            new SavingGoal(1, "Goal 1", 1000, DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), userId)
        };
        repoMock.Setup(r => r.FindByUserIdPaged(userId, page, pageSize)).Returns(goals);
        repoMock.Setup(r => r.FindByUserId(userId)).Returns(goals);
        transactionServiceMock.Setup(t => t.GetBalance(userId)).Returns(1000);
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var result = service.GetByUserIdPaged(userId, page, pageSize);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Goal 1", result[0].Name);
    }

    [TestMethod]
    [DataRow(4, 1, 10)]
    [DataRow(12, 1, 10)]
    public void GetByUserIdPaged_SavingGoalNotFoundException(int userId, int page, int pageSize) {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.FindByUserIdPaged(userId, page, pageSize)).Returns(new List<SavingGoal>());
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<SavingGoalNotFoundException>(() =>
            service.GetByUserIdPaged(userId, page, pageSize));
        Assert.AreEqual($"Saving goal with user id {userId} not found.", ex.Message);
    }

    [TestMethod]
    public void GetByUserIdPaged_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.FindByUserIdPaged(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Throws(new DatabaseException("DB error"));
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetByUserIdPaged(1, 1, 10));
        Assert.AreEqual("Database error retrieving paged saving goals for user_id: 1", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    [DataRow("", 1000, "2025-12-31", 1)]
    [DataRow("   ", 1000, "2025-12-31", 1)]
    public void Add_NameIsEmpty_ThrowsInvalidDataException(string name, double target, string deadlineStr, int userId) {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.Add(It.IsAny<SavingGoal>()))
            .Throws(new ArgumentException("Name cannot be empty."));
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var deadline = DateOnly.Parse(deadlineStr);

        var ex = Assert.ThrowsException<InvalidDataException>(() => service.Add(name, target, deadline, userId));
        Assert.AreEqual("Invalid saving goal data: Name cannot be empty.", ex.Message);
    }

    [TestMethod]
    public void Add_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.Add(It.IsAny<SavingGoal>())).Throws(new DatabaseException("DB error"));
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() =>
            service.Add("Test", 1000, DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1)
        );
        Assert.AreEqual("Database error while adding a saving goal.", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    public void Add_ValidSavingGoal_ReturnsDto() {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        var goal = new SavingGoal(1, "Test Goal", 1000, DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1);
        repoMock.Setup(r => r.Add(It.IsAny<SavingGoal>())).Returns(goal);
        repoMock.Setup(r => r.FindByUserId(goal.UserId)).Returns(new List<SavingGoal> { goal });
        transactionServiceMock.Setup(t => t.GetBalance(goal.UserId)).Returns(500);
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var result = service.Add("Test Goal", 1000, goal.Deadline, goal.UserId);

        Assert.IsNotNull(result);
        Assert.AreEqual("Test Goal", result.Name);
        Assert.AreEqual(500, result.AmountSaved);
        Assert.AreEqual(1000, result.Target);
    }

    [TestMethod]
    public void Edit_SavingGoalNotFound_ReturnsFalse() {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.Edit(It.IsAny<SavingGoal>())).Throws(new SavingGoalNotFoundException("Not found"));
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var result = service.Edit(new SavingGoal(1, "Test", 1000, DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1));
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Edit_DatabaseException_ReturnsFalse() {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.Edit(It.IsAny<SavingGoal>())).Throws(new DatabaseException("DB error"));
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var result = service.Edit(new SavingGoal(1, "Test", 1000, DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1));
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Edit_ValidSavingGoal_ReturnsTrue() {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.Edit(It.IsAny<SavingGoal>())).Returns(true);
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var result = service.Edit(new SavingGoal(1, "Test", 1000, DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1));
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Delete_IdZeroOrNegative_ReturnsFalse(int goalId) {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.FindById(goalId)).Throws(new ArgumentException("Invalid ID"));
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var result = service.Delete(goalId);
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void Delete_SavingGoalNotFound_ReturnsFalse(int goalId) {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        repoMock.Setup(r => r.FindById(goalId)).Throws(new SavingGoalNotFoundException("Not found"));
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var result = service.Delete(goalId);
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void Delete_ValidSavingGoal_ReturnsTrue(int goalId) {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        var goal = new SavingGoal(goalId, "Test", 1000, DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1);
        repoMock.Setup(r => r.FindById(goalId)).Returns(goal);
        repoMock.Setup(r => r.Delete(goal)).Returns(true);
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var result = service.Delete(goalId);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Delete_DatabaseException_ReturnsFalse() {
        var loggerMock = new Mock<ILogger<SavingGoalService>>();
        var repoMock = new Mock<ISavingGoalRepository>();
        var transactionServiceMock = new Mock<ITransactionService>();
        var goal = new SavingGoal(1, "Test", 1000, DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1);
        repoMock.Setup(r => r.FindById(1)).Returns(goal);
        repoMock.Setup(r => r.Delete(It.IsAny<SavingGoal>())).Throws(new DatabaseException("DB error"));
        var service = new SavingGoalService(repoMock.Object, transactionServiceMock.Object, loggerMock.Object);

        var result = service.Delete(1);
        Assert.IsFalse(result);
    }
}