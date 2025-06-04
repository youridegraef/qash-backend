using Application.Domain;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestLayer;

[TestClass]
public class BudgetTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetById_IdZeroOrNegative_ThrowsException(int budgetId) {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.FindById(budgetId))
            .Throws(new ArgumentException($"Budget ID must be greater than zero: {budgetId}"));
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetById(budgetId));
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
        Assert.AreEqual($"Error retrieving budget with id: {budgetId}", ex.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void GetById_BudgetNotFoundException(int budgetId) {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.FindById(budgetId)).Returns((Budget)null!);
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<BudgetNotFoundException>(() => service.GetById(budgetId));
        Assert.AreEqual($"No budget with id: {budgetId} found.", ex.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetById_ReturnsBudgetDto(int budgetId) {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var budget = new Budget(budgetId, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1000, 1);
        var category = new Category(1, "Test Category", 1);
        repoMock.Setup(r => r.FindById(budgetId)).Returns(budget);
        categoryServiceMock.Setup(c => c.GetById(budget.CategoryId)).Returns(category);
        repoMock.Setup(r => r.CalculateBudgetSpending(budgetId)).Returns(500);
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var result = service.GetById(budgetId);

        Assert.IsNotNull(result);
        Assert.AreEqual(budgetId, result.Id);
        Assert.AreEqual("Test Category", result.Name);
        Assert.AreEqual(500, result.Spent);
        Assert.AreEqual(1000, result.Target);
    }

    [TestMethod]
    public void GetById_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.FindById(It.IsAny<int>())).Throws(new DatabaseException("DB error"));
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetById(1));
        Assert.AreEqual("Database error retrieving budget with id: 1", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetByUserId_IdZeroOrNegative_ThrowsException(int userId) {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.FindByUserId(userId))
            .Throws(new ArgumentException($"User ID must be greater than zero: {userId}"));
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetByUserId(userId));
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
        Assert.AreEqual($"Error retrieving budgets for user with id: {userId}", ex.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetByUserId_ReturnsBudgetDtoList(int userId) {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var budgets = new List<Budget> {
            new Budget(1, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1000,
                1)
        };
        var category = new Category(1, "Test Category", userId);
        repoMock.Setup(r => r.FindByUserId(userId)).Returns(budgets);
        categoryServiceMock.Setup(c => c.GetById(1)).Returns(category);
        repoMock.Setup(r => r.CalculateBudgetSpending(It.IsAny<int>())).Returns(500);
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var result = service.GetByUserId(userId);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Test Category", result[0].Name);
        Assert.AreEqual(500, result[0].Spent);
    }

    [TestMethod]
    public void GetByUserId_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.FindByUserId(It.IsAny<int>())).Throws(new DatabaseException("DB error"));
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetByUserId(1));
        Assert.AreEqual("Database error retrieving budgets for user with id: 1", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetByCategoryId_IdZeroOrNegative_ThrowsException(int categoryId) {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.FindByCategoryId(categoryId))
            .Throws(new ArgumentException($"Category ID must be greater than zero: {categoryId}"));
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetByCategoryId(categoryId));
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
        Assert.AreEqual($"Error retrieving budget with category id: {categoryId}", ex.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void GetByCategoryId_BudgetNotFoundException(int categoryId) {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.FindByCategoryId(categoryId)).Returns((Budget)null!);
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<BudgetNotFoundException>(() => service.GetByCategoryId(categoryId));
        Assert.AreEqual($"No budget with category id: {categoryId} found.", ex.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetByCategoryId_ReturnsBudgetDto(int categoryId) {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var budget = new Budget(1, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1000, categoryId);
        var category = new Category(categoryId, "Test Category", 1);
        repoMock.Setup(r => r.FindByCategoryId(categoryId)).Returns(budget);
        categoryServiceMock.Setup(c => c.GetById(categoryId)).Returns(category);
        repoMock.Setup(r => r.CalculateBudgetSpending(budget.Id)).Returns(500);
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var result = service.GetByCategoryId(categoryId);

        Assert.IsNotNull(result);
        Assert.AreEqual("Test Category", result.Name);
        Assert.AreEqual(500, result.Spent);
    }

    [TestMethod]
    public void GetByCategoryId_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.FindByCategoryId(It.IsAny<int>())).Throws(new DatabaseException("DB error"));
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetByCategoryId(1));
        Assert.AreEqual("Database error retrieving budget with category id: 1", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    [DataRow("", "2025-12-31", "2026-12-31", 1000, 1)]
    [DataRow("   ", "2025-12-31", "2026-12-31", 1000, 1)]
    public void Add_NameIsEmpty_ThrowsInvalidDataException(string name, string startDateStr, string endDateStr,
        double target, int categoryId) {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.Add(It.IsAny<Budget>()))
            .Throws(new ArgumentException("Name cannot be empty."));
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var startDate = DateOnly.Parse(startDateStr);
        var endDate = DateOnly.Parse(endDateStr);

        var ex = Assert.ThrowsException<InvalidDataException>(() =>
            service.Add(startDate, endDate, target, categoryId));
        Assert.AreEqual("Invalid budget data: Name cannot be empty.", ex.Message);
    }

    [TestMethod]
    public void Add_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.Add(It.IsAny<Budget>())).Throws(new DatabaseException("DB error"));
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() =>
            service.Add(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1000, 1)
        );
        Assert.AreEqual("Database error while adding a budget.", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    public void Add_ValidBudget_ReturnsBudgetDto() {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var endDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));
        var budget = new Budget(1, startDate, endDate, 1000, 1);
        var category = new Category(1, "Test Category", 1);
        repoMock.Setup(r => r.Add(It.IsAny<Budget>())).Returns(budget);
        categoryServiceMock.Setup(c => c.GetById(1)).Returns(category);
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var result = service.Add(startDate, endDate, 1000, 1);

        Assert.IsNotNull(result);
        Assert.AreEqual("Test Category", result.Name);
        Assert.AreEqual(0, result.Spent);
        Assert.AreEqual(1000, result.Target);
    }

    [TestMethod]
    public void Edit_BudgetNotFound_ReturnsFalse() {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.Edit(It.IsAny<Budget>())).Throws(new BudgetNotFoundException("Not found"));
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var result = service.Edit(new Budget(1, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1000, 1));
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Edit_DatabaseException_ReturnsFalse() {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.Edit(It.IsAny<Budget>())).Throws(new DatabaseException("DB error"));
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var result = service.Edit(new Budget(1, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1000, 1));
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Edit_ValidBudget_ReturnsTrue() {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.Edit(It.IsAny<Budget>())).Returns(true);
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var result = service.Edit(new Budget(1, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1000, 1));
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Delete_IdZeroOrNegative_ReturnsFalse(int budgetId) {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.FindById(budgetId)).Throws(new ArgumentException("Invalid ID"));
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var result = service.Delete(budgetId);
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void Delete_BudgetNotFound_ReturnsFalse(int budgetId) {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        repoMock.Setup(r => r.FindById(budgetId)).Throws(new BudgetNotFoundException("Not found"));
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var result = service.Delete(budgetId);
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void Delete_ValidBudget_ReturnsTrue(int budgetId) {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var budget = new Budget(budgetId, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1000, 1);
        repoMock.Setup(r => r.FindById(budgetId)).Returns(budget);
        repoMock.Setup(r => r.Delete(budget)).Returns(true);
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var result = service.Delete(budgetId);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Delete_DatabaseException_ReturnsFalse() {
        var loggerMock = new Mock<ILogger<BudgetService>>();
        var repoMock = new Mock<IBudgetRepository>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var budget = new Budget(1, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), 1000, 1);
        repoMock.Setup(r => r.FindById(1)).Returns(budget);
        repoMock.Setup(r => r.Delete(It.IsAny<Budget>())).Throws(new DatabaseException("DB error"));
        var service = new BudgetService(repoMock.Object, categoryServiceMock.Object, loggerMock.Object);

        var result = service.Delete(1);
        Assert.IsFalse(result);
    }
}