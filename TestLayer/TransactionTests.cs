using System.Security.Authentication;
using Application.Exceptions;
using Application.Services;
using Application.Interfaces;
using Application.Domain;
using Application.Dtos;
using Moq;

namespace TestLayer;

[TestClass]
public class TransactionTests {
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetById_IdZeroOrNegativeException(int transactionId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();

        var originalExceptionMessage = $"Transaction ID must be greater than zero: {transactionId}";
        transactionRepoMock.Setup(r => r.FindById(transactionId))
            .Throws(new ArgumentException(originalExceptionMessage));

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var exception = Assert.ThrowsException<Exception>(() =>
            transactionService.GetById(transactionId)
        );

        // Assert
        Assert.AreEqual(
            $"Error retrieving transaction with id: {transactionId}",
            exception.Message
        );
        Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
        Assert.AreEqual(originalExceptionMessage, exception.InnerException.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void GetById_TransactionNotFoundException(int transactionId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        transactionRepoMock
            .Setup(r => r.FindById(transactionId))
            .Returns((Transaction)null!);
        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var exception = Assert.ThrowsException<TransactionNotFoundException>(() =>
            transactionService.GetById(transactionId)
        );

        // Assert
        Assert.AreEqual(
            $"No transaction with id: {transactionId} found.",
            exception.Message
        );
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetById_ReturnsTransactionDto(int transactionId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var transaction = new Transaction(
            transactionId,
            "Test Transaction",
            100.0,
            DateOnly.FromDateTime(DateTime.Now),
            1,
            1
        );
        var category = new Category(1, "Test Category", 1, "#FF0000");
        var tags = new List<Tag> { new Tag(1, "Test Tag") };

        transactionRepoMock.Setup(r => r.FindById(transactionId)).Returns(transaction);
        categoryServiceMock.Setup(c => c.GetById(1)).Returns(category);
        tagServiceMock.Setup(t => t.GetByTransactionId(transactionId)).Returns(tags);

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = transactionService.GetById(transactionId);

        // Assert
        Assert.IsNotNull(result, "TransactionDto should not be null");
        Assert.AreEqual(transactionId, result.Id, "Transaction id should match");
        Assert.AreEqual("Test Transaction", result.Description, "Description should match");
    }

    [TestMethod]
    public void GetById_DatabaseException_IsWrapped() {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        transactionRepoMock
            .Setup(r => r.FindById(It.IsAny<int>()))
            .Throws(new DatabaseException("DB error"));
        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var ex = Assert.ThrowsException<Exception>(() => transactionService.GetById(1));

        // Assert
        Assert.AreEqual("Database error retrieving transaction with id: 1", ex.Message);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetByUserId_IdZeroOrNegativeException(int userId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();

        var originalExceptionMessage = $"User ID must be greater than zero: {userId}";
        transactionRepoMock.Setup(r => r.FindByUserId(userId))
            .Throws(new ArgumentException(originalExceptionMessage));

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var exception = Assert.ThrowsException<Exception>(() =>
            transactionService.GetByUserId(userId)
        );

        // Assert
        Assert.AreEqual(
            $"No transactions found with user_id: {userId}",
            exception.Message
        );
        Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
        Assert.AreEqual(originalExceptionMessage, exception.InnerException.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void GetByUserId_TransactionNotFoundException(int userId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        transactionRepoMock
            .Setup(r => r.FindByUserId(userId))
            .Returns(new List<Transaction>());
        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var exception = Assert.ThrowsException<TransactionNotFoundException>(() =>
            transactionService.GetByUserId(userId)
        );

        // Assert
        Assert.AreEqual(
            $"No transaction with user id: {userId} found.",
            exception.Message
        );
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetByUserId_ReturnsTransactionDtoList(int userId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var transactions = new List<Transaction> {
            new Transaction(1, "Test Transaction 1", 100.0, DateOnly.FromDateTime(DateTime.Now), userId, 1),
            new Transaction(2, "Test Transaction 2", 200.0, DateOnly.FromDateTime(DateTime.Now), userId, 1)
        };
        var category = new Category(1, "Test Category", userId, "#ABCDEF");
        var tags = new List<Tag> { new Tag(1, "Test Tag") };

        transactionRepoMock.Setup(r => r.FindByUserId(userId)).Returns(transactions);
        categoryServiceMock.Setup(c => c.GetById(1)).Returns(category);
        tagServiceMock.Setup(t => t.GetByTransactionId(It.IsAny<int>())).Returns(tags);

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = transactionService.GetByUserId(userId);

        // Assert
        Assert.IsNotNull(result, "TransactionDto list should not be null");
        Assert.AreEqual(2, result.Count, "Should return 2 transactions");
        Assert.AreEqual(userId, result[0].UserId, "User id should match");
    }

    [TestMethod]
    public void GetByUserId_DatabaseException_IsWrapped() {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        transactionRepoMock
            .Setup(r => r.FindByUserId(It.IsAny<int>()))
            .Throws(new DatabaseException("DB error"));
        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var ex = Assert.ThrowsException<Exception>(() => transactionService.GetByUserId(1));

        // Assert
        Assert.AreEqual(
            "Database error while retrieving transactions with user_id: 1",
            ex.Message
        );
    }

    [TestMethod]
    [DataRow(-1, 1, 10)]
    [DataRow(0, 1, 10)]
    public void GetByUserIdPaged_IdZeroOrNegativeException(int userId, int page, int pageSize) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();

        var originalExceptionMessage = $"User ID must be greater than zero: {userId}";
        transactionRepoMock.Setup(r => r.FindByUserIdPaged(userId, page, pageSize))
            .Throws(new ArgumentException(originalExceptionMessage));

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var exception = Assert.ThrowsException<Exception>(() =>
            transactionService.GetByUserIdPaged(userId, page, pageSize)
        );

        // Assert
        Assert.AreEqual(
            $"No transactions found. {originalExceptionMessage}",
            exception.Message
        );
        Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
        Assert.AreEqual(originalExceptionMessage, exception.InnerException.Message);
    }

    [TestMethod]
    [DataRow(1, -1, 10)]
    [DataRow(1, 0, 10)]
    public void GetByUserIdPaged_PageZeroOrNegativeException(int userId, int page, int pageSize) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();

        var originalExceptionMessage = $"Page must be greater than zero: {page}";
        transactionRepoMock.Setup(r => r.FindByUserIdPaged(userId, page, pageSize))
            .Throws(new ArgumentException(originalExceptionMessage));

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var exception = Assert.ThrowsException<Exception>(() =>
            transactionService.GetByUserIdPaged(userId, page, pageSize)
        );

        // Assert
        Assert.AreEqual(
            $"No transactions found. {originalExceptionMessage}",
            exception.Message
        );
        Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
        Assert.AreEqual(originalExceptionMessage, exception.InnerException.Message);
    }

    [TestMethod]
    [DataRow(1, 1, -1)]
    [DataRow(1, 1, 0)]
    public void GetByUserIdPaged_PageSizeZeroOrNegativeException(
        int userId,
        int page,
        int pageSize
    ) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();

        var originalExceptionMessage = $"Page size must be greater than zero: {pageSize}";
        transactionRepoMock.Setup(r => r.FindByUserIdPaged(userId, page, pageSize))
            .Throws(new ArgumentException(originalExceptionMessage));

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var exception = Assert.ThrowsException<Exception>(() =>
            transactionService.GetByUserIdPaged(userId, page, pageSize)
        );

        // Assert
        Assert.AreEqual(
            $"No transactions found. {originalExceptionMessage}",
            exception.Message
        );
        Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
        Assert.AreEqual(originalExceptionMessage, exception.InnerException.Message);
    }

    [TestMethod]
    [DataRow(4, 1, 10)]
    [DataRow(12, 1, 10)]
    public void GetByUserIdPaged_TransactionNotFoundException(
        int userId,
        int page,
        int pageSize
    ) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        transactionRepoMock
            .Setup(r => r.FindByUserIdPaged(userId, page, pageSize))
            .Returns(new List<Transaction>());
        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var exception = Assert.ThrowsException<TransactionNotFoundException>(() =>
            transactionService.GetByUserIdPaged(userId, page, pageSize)
        );

        // Assert
        Assert.AreEqual(
            $"No transaction with user id: {userId} found.",
            exception.Message
        );
    }

    [TestMethod]
    [DataRow(1, 1, 10)]
    [DataRow(2, 1, 5)]
    public void GetByUserIdPaged_ReturnsTransactionDtoList(
        int userId,
        int page,
        int pageSize
    ) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var transactions = new List<Transaction> {
            new Transaction(1, "Test Transaction 1", 100.0, DateOnly.FromDateTime(DateTime.Now), userId, 1)
        };
        var category = new Category(1, "Test Category", userId, "#FEDCBA");
        var tags = new List<Tag> { new Tag(1, "Test Tag") };

        transactionRepoMock
            .Setup(r => r.FindByUserIdPaged(userId, page, pageSize))
            .Returns(transactions);
        categoryServiceMock.Setup(c => c.GetById(1)).Returns(category);
        tagServiceMock.Setup(t => t.GetByTransactionId(It.IsAny<int>())).Returns(tags);

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = transactionService.GetByUserIdPaged(userId, page, pageSize);

        // Assert
        Assert.IsNotNull(result, "TransactionDto list should not be null");
        Assert.AreEqual(1, result.Count, "Should return 1 transaction");
        Assert.AreEqual(userId, result[0].UserId, "User id should match");
    }

    [TestMethod]
    public void GetByUserIdPaged_DatabaseException_IsWrapped() {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        transactionRepoMock
            .Setup(r => r.FindByUserIdPaged(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Throws(new DatabaseException("DB error"));
        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var ex = Assert.ThrowsException<Exception>(() =>
            transactionService.GetByUserIdPaged(1, 1, 10)
        );

        // Assert
        Assert.AreEqual(
            "Database error while retrieving paged transactions.",
            ex.Message
        );
    }

    [TestMethod]
    [DataRow("", 100.0, 1, 1)]
    [DataRow("   ", 100.0, 1, 1)]
    public void Add_DescriptionIsEmptyException(
        string description,
        double amount,
        int userId,
        int categoryId
    ) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();

        var originalArgExMessage = "Description cannot be empty.";
        transactionRepoMock.Setup(r => r.Add(It.IsAny<Transaction>()))
            .Throws(new ArgumentException(originalArgExMessage));


        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );
        var date = DateOnly.FromDateTime(DateTime.Now);
        var tags = new List<Tag>();

        // Act
        var exception = Assert.ThrowsException<InvalidDataException>(() =>
            transactionService.Add(description, amount, date, userId, categoryId, tags)
        );

        // Assert
        Assert.AreEqual(
            $"Invalid transaction data: {originalArgExMessage}",
            exception.Message
        );
    }

    [TestMethod]
    [DataRow("Test Transaction", 100.0, -1, 1)]
    [DataRow("Test Transaction", 100.0, 0, 1)]
    public void Add_UserIdZeroOrNegativeException(
        string description,
        double amount,
        int userId,
        int categoryId
    ) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();

        var originalArgExMessage = "User ID must be greater than zero.";
        transactionRepoMock.Setup(r => r.Add(It.IsAny<Transaction>()))
            .Throws(new ArgumentException(originalArgExMessage));

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );
        var date = DateOnly.FromDateTime(DateTime.Now);
        var tags = new List<Tag>();

        // Act
        var exception = Assert.ThrowsException<InvalidDataException>(() =>
            transactionService.Add(description, amount, date, userId, categoryId, tags)
        );

        // Assert
        Assert.AreEqual(
            $"Invalid transaction data: {originalArgExMessage}",
            exception.Message
        );
    }

    [TestMethod]
    [DataRow("Test Transaction", 100.0, 1, -1)]
    [DataRow("Test Transaction", 100.0, 1, 0)]
    public void Add_CategoryIdZeroOrNegativeException(
        string description,
        double amount,
        int userId,
        int categoryId
    ) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();

        var originalArgExMessage = "Category ID must be greater than zero.";
        transactionRepoMock.Setup(r => r.Add(It.IsAny<Transaction>()))
            .Throws(new ArgumentException(originalArgExMessage));

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );
        var date = DateOnly.FromDateTime(DateTime.Now);
        var tags = new List<Tag>();

        // Act
        var exception = Assert.ThrowsException<InvalidDataException>(() =>
            transactionService.Add(description, amount, date, userId, categoryId, tags)
        );

        // Assert
        Assert.AreEqual(
            $"Invalid transaction data: {originalArgExMessage}",
            exception.Message
        );
    }

    [TestMethod]
    [DataRow("Test Transaction", 100.0, 1, 1)]
    public void Add_ValidTransaction_ReturnsTransactionDto(
        string description,
        double amount,
        int userId,
        int categoryId
    ) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var date = DateOnly.FromDateTime(DateTime.Now);
        var tags = new List<Tag>();

        var addedTransaction = new Transaction(
            1,
            description,
            amount,
            date,
            userId,
            categoryId
        );
        var category = new Category(categoryId, "Test Category", userId, "#112233");

        transactionRepoMock
            .Setup(r => r.Add(It.IsAny<Transaction>()))
            .Returns(addedTransaction);

        categoryServiceMock.Setup(c => c.GetById(categoryId)).Returns(category);

        tagServiceMock.Setup(t => t.GetByTransactionId(addedTransaction.Id)).Returns(tags);

        transactionRepoMock
            .Setup(r => r.AddTagsToTransaction(addedTransaction.Id, tags))
            .Verifiable();

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = transactionService.Add(
            description,
            amount,
            date,
            userId,
            categoryId,
            tags
        );

        // Assert
        Assert.IsNotNull(result, "TransactionDto should not be null");
        Assert.AreEqual(description, result.Description, "Description should match");
        Assert.AreEqual(amount, result.Amount, "Amount should match");
        Assert.AreEqual(userId, result.UserId, "User id should match");
        Assert.AreEqual(addedTransaction.Id, result.Id, "Returned DTO ID should match added transaction ID");

        transactionRepoMock.Verify(r => r.AddTagsToTransaction(addedTransaction.Id, tags), Times.Once);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Edit_IdZeroOrNegativeReturnsFalse(int transactionId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );
        var date = DateOnly.FromDateTime(DateTime.Now);
        var tags = new List<Tag>();

        // Act
        var result = transactionService.Edit(
            transactionId,
            100.0,
            "Test",
            date,
            1,
            1,
            tags
        );

        // Assert
        Assert.IsFalse(result, "Edit should return false for zero or negative id");
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void Edit_ValidTransaction_ReturnsTrue(int transactionId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        transactionRepoMock.Setup(r => r.Edit(It.IsAny<Transaction>())).Returns(true);
        transactionRepoMock.Setup(r => r.AddTagsToTransaction(transactionId, It.IsAny<List<Tag>>()));

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );
        var date = DateOnly.FromDateTime(DateTime.Now);
        var tags = new List<Tag>();

        // Act
        var result = transactionService.Edit(
            transactionId,
            100.0,
            "Test",
            date,
            1,
            1,
            tags
        );

        // Assert
        Assert.IsTrue(result, "Edit should return true for valid transaction");
    }

    [TestMethod]
    public void Edit_DatabaseException_ReturnsFalse() {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        transactionRepoMock
            .Setup(r => r.Edit(It.IsAny<Transaction>()))
            .Throws(new DatabaseException("DB error"));
        transactionRepoMock.Setup(r => r.AddTagsToTransaction(It.IsAny<int>(), It.IsAny<List<Tag>>()));

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );
        var date = DateOnly.FromDateTime(DateTime.Now);
        var tags = new List<Tag>();

        // Act
        var result = transactionService.Edit(1, 100.0, "Test", date, 1, 1, tags);

        // Assert
        Assert.IsFalse(result, "Edit should return false on database exception");
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Delete_IdZeroOrNegative_ReturnsFalse(int transactionId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();

        transactionRepoMock.Setup(r => r.FindById(transactionId))
            .Throws(new ArgumentException("Invalid ID"));

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = transactionService.Delete(transactionId);

        // Assert
        Assert.IsFalse(
            result,
            "Delete should return false for zero or negative transactionId"
        );
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void Delete_TransactionNotFound_ReturnsFalse(int transactionId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        transactionRepoMock
            .Setup(r => r.FindById(transactionId))
            .Throws(new TransactionNotFoundException("Not found"));
        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = transactionService.Delete(transactionId);

        // Assert
        Assert.IsFalse(result, "Delete should return false if transaction is not found");
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void Delete_ValidTransaction_ReturnsTrue(int transactionId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var transaction = new Transaction(
            transactionId,
            "Test Transaction",
            100.0,
            DateOnly.FromDateTime(DateTime.Now),
            1,
            1
        );
        transactionRepoMock.Setup(r => r.FindById(transactionId)).Returns(transaction);
        transactionRepoMock.Setup(r => r.Delete(It.IsAny<Transaction>())).Returns(true);
        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = transactionService.Delete(transactionId);

        // Assert
        Assert.IsTrue(result, "Delete should return true for valid transaction");
    }

    [TestMethod]
    public void Delete_DatabaseException_ReturnsFalse() {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var transaction = new Transaction(
            1,
            "Test Transaction",
            100.0,
            DateOnly.FromDateTime(DateTime.Now),
            1,
            1
        );
        transactionRepoMock.Setup(r => r.FindById(1)).Returns(transaction);
        transactionRepoMock
            .Setup(r => r.Delete(It.IsAny<Transaction>()))
            .Throws(new DatabaseException("DB error"));
        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = transactionService.Delete(1);

        // Assert
        Assert.IsFalse(result, "Delete should return false on database exception");
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetBalance_IdZeroOrNegativeException(int userId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();

        var originalExceptionMessage = $"User ID must be greater than zero: {userId}";
        transactionRepoMock.Setup(r => r.FindByUserId(userId))
            .Throws(new ArgumentException(originalExceptionMessage));

        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var exception = Assert.ThrowsException<Exception>(() =>
            transactionService.GetBalance(userId)
        );

        // Assert
        Assert.AreEqual(
            $"Error calculating balance for user_id: {userId}",
            exception.Message
        );
        Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
        Assert.AreEqual(originalExceptionMessage, exception.InnerException.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetBalance_ReturnsCorrectBalance(int userId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var transactions = new List<Transaction> {
            new Transaction(1, "Income", 100.0, DateOnly.FromDateTime(DateTime.Now), userId, 1),
            new Transaction(2, "Expense", -50.0, DateOnly.FromDateTime(DateTime.Now), userId, 1)
        };
        transactionRepoMock.Setup(r => r.FindByUserId(userId)).Returns(transactions);
        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = transactionService.GetBalance(userId);

        // Assert
        Assert.AreEqual(50.0, result, "Balance should be 50.0");
    }

    [TestMethod]
    public void GetBalance_DatabaseException_IsWrapped() {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        transactionRepoMock
            .Setup(r => r.FindByUserId(It.IsAny<int>()))
            .Throws(new DatabaseException("DB error"));
        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var ex = Assert.ThrowsException<Exception>(() => transactionService.GetBalance(1));

        // Assert
        Assert.AreEqual(
            "Database error while calculating balance for user_id: 1",
            ex.Message
        );
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetExpenses_ReturnsCorrectExpenses(int userId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var transactions = new List<Transaction> {
            new Transaction(1, "Income", 100.0, DateOnly.FromDateTime(DateTime.Now), userId, 1),
            new Transaction(2, "Expense", -50.0, DateOnly.FromDateTime(DateTime.Now), userId, 1),
            new Transaction(3, "Expense", -30.0, DateOnly.FromDateTime(DateTime.Now), userId, 1)
        };
        transactionRepoMock.Setup(r => r.FindByUserId(userId)).Returns(transactions);
        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = transactionService.GetExpenses(userId);

        // Assert
        Assert.AreEqual(80.0, result, "Expenses should be 80.0");
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetIncome_ReturnsCorrectIncome(int userId) {
        // Arrange
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var tagServiceMock = new Mock<ITagService>();
        var categoryServiceMock = new Mock<ICategoryService>();
        var transactions = new List<Transaction> {
            new Transaction(1, "Income", 100.0, DateOnly.FromDateTime(DateTime.Now), userId, 1),
            new Transaction(2, "Income", 200.0, DateOnly.FromDateTime(DateTime.Now), userId, 1),
            new Transaction(3, "Expense", -50.0, DateOnly.FromDateTime(DateTime.Now), userId, 1)
        };
        transactionRepoMock.Setup(r => r.FindByUserId(userId)).Returns(transactions);
        var transactionService = new TransactionService(
            transactionRepoMock.Object,
            tagServiceMock.Object,
            categoryServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = transactionService.GetIncome(userId);

        // Assert
        Assert.AreEqual(300.0, result, "Income should be 300.0");
    }
}