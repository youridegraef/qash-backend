using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestLayer;

[TestClass]
public class CategoryTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetById_IdZeroOrNegative_ThrowsException(int categoryId) {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.FindById(categoryId))
            .Throws(new ArgumentException($"Category ID must be greater than zero: {categoryId}"));
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetById(categoryId));
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
        Assert.AreEqual($"Error retrieving category with id: {categoryId}", ex.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void GetById_CategoryNotFoundException(int categoryId) {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.FindById(categoryId)).Returns((Category)null!);
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<CategoryNotFoundException>(() => service.GetById(categoryId));
        Assert.AreEqual($"No category with id: {categoryId} found.", ex.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetById_ReturnsCategory(int categoryId) {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        var category = new Category(categoryId, "Test Category", 1);
        repoMock.Setup(r => r.FindById(categoryId)).Returns(category);
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var result = service.GetById(categoryId);

        Assert.IsNotNull(result);
        Assert.AreEqual(categoryId, result.Id);
        Assert.AreEqual("Test Category", result.Name);
    }

    [TestMethod]
    public void GetById_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.FindById(It.IsAny<int>())).Throws(new DatabaseException("DB error"));
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetById(1));
        Assert.AreEqual("Database error retrieving category with id: 1", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetByUserId_IdZeroOrNegative_ThrowsException(int userId) {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.FindByUserId(userId))
            .Throws(new ArgumentException($"User ID must be greater than zero: {userId}"));
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetByUserId(userId));
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
        Assert.AreEqual($"Error retrieving categories for user_id: {userId}", ex.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void GetByUserId_NoCategoriesFound_ThrowsException(int userId) {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.FindByUserId(userId)).Throws(new KeyNotFoundException());
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetByUserId(userId));
        Assert.AreEqual($"No categories found for user_id: {userId}", ex.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetByUserId_ReturnsCategoryList(int userId) {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        var categories = new List<Category> { new Category(1, "Test Category", userId) };
        repoMock.Setup(r => r.FindByUserId(userId)).Returns(categories);
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var result = service.GetByUserId(userId);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Test Category", result[0].Name);
    }

    [TestMethod]
    public void GetByUserId_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.FindByUserId(It.IsAny<int>())).Throws(new DatabaseException("DB error"));
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetByUserId(1));
        Assert.AreEqual("Database error retrieving categories for user_id: 1", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    [DataRow(1, 1, 10)]
    [DataRow(2, 1, 5)]
    public void GetByUserIdPaged_ReturnsCategoryList(int userId, int page, int pageSize) {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        var categories = new List<Category> { new Category(1, "Test Category", userId) };
        repoMock.Setup(r => r.FindByUserIdPaged(userId, page, pageSize)).Returns(categories);
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var result = service.GetByUserIdPaged(userId, page, pageSize);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Test Category", result[0].Name);
    }

    [TestMethod]
    [DataRow(4, 1, 10)]
    [DataRow(12, 1, 10)]
    public void GetByUserIdPaged_NoCategoriesFound_ThrowsException(int userId, int page, int pageSize) {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.FindByUserIdPaged(userId, page, pageSize)).Throws(new KeyNotFoundException());
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetByUserIdPaged(userId, page, pageSize));
        Assert.AreEqual($"No categories found for user_id: {userId}", ex.Message);
    }

    [TestMethod]
    public void GetByUserIdPaged_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.FindByUserIdPaged(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Throws(new DatabaseException("DB error"));
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.GetByUserIdPaged(1, 1, 10));
        Assert.AreEqual("Database error retrieving paged categories for user_id: 1", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    [DataRow("", 1)]
    [DataRow("   ", 1)]
    public void Add_NameIsEmpty_ThrowsInvalidDataException(string name, int userId) {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.Add(It.IsAny<Category>()))
            .Throws(new ArgumentException("Name cannot be empty."));
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<InvalidDataException>(() => service.Add(name, userId));
        Assert.AreEqual("Invalid category data: Name cannot be empty.", ex.Message);
    }

    [TestMethod]
    public void Add_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.Add(It.IsAny<Category>())).Throws(new DatabaseException("DB error"));
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => service.Add("Test", 1));
        Assert.AreEqual("Database error while adding a category.", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    public void Add_ValidCategory_ReturnsCategory() {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        var category = new Category(1, "Test Category", 1);
        repoMock.Setup(r => r.Add(It.IsAny<Category>())).Returns(category);
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var result = service.Add("Test Category", 1);

        Assert.IsNotNull(result);
        Assert.AreEqual("Test Category", result.Name);
        Assert.AreEqual(1, result.UserId);
    }

    [TestMethod]
    public void Edit_CategoryNotFound_ReturnsFalse() {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.Edit(It.IsAny<Category>())).Throws(new CategoryNotFoundException("Not found"));
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var result = service.Edit(new Category(1, "Test", 1));
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Edit_DatabaseException_ReturnsFalse() {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.Edit(It.IsAny<Category>())).Throws(new DatabaseException("DB error"));
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var result = service.Edit(new Category(1, "Test", 1));
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Edit_ValidCategory_ReturnsTrue() {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.Edit(It.IsAny<Category>())).Returns(true);
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var result = service.Edit(new Category(1, "Test", 1));
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Delete_IdZeroOrNegative_ReturnsFalse(int categoryId) {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.FindById(categoryId)).Throws(new ArgumentException("Invalid ID"));
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var result = service.Delete(categoryId);
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void Delete_CategoryNotFound_ReturnsFalse(int categoryId) {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        repoMock.Setup(r => r.FindById(categoryId)).Throws(new CategoryNotFoundException("Not found"));
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var result = service.Delete(categoryId);
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void Delete_ValidCategory_ReturnsTrue(int categoryId) {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        var category = new Category(categoryId, "Test", 1);
        repoMock.Setup(r => r.FindById(categoryId)).Returns(category);
        repoMock.Setup(r => r.Delete(category)).Returns(true);
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var result = service.Delete(categoryId);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Delete_DatabaseException_ReturnsFalse() {
        var loggerMock = new Mock<ILogger<CategoryService>>();
        var repoMock = new Mock<ICategoryRepository>();
        var category = new Category(1, "Test", 1);
        repoMock.Setup(r => r.FindById(1)).Returns(category);
        repoMock.Setup(r => r.Delete(It.IsAny<Category>())).Throws(new DatabaseException("DB error"));
        var service = new CategoryService(repoMock.Object, loggerMock.Object);

        var result = service.Delete(1);
        Assert.IsFalse(result);
    }
}