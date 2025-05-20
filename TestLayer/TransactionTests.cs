using Moq;
using Microsoft.Extensions.Logging;
using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using Application.Services;

namespace TestLayer;

[TestClass]
public class TransactionTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetById_IdZeroOrNegative_ThrowsExceptionWithMessage(int transactionId)
    {
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var transactionService = new TransactionService(transactionRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<TransactionNotFoundException>(() =>
            transactionService.GetById(transactionId)
        );
        Assert.AreEqual($"No transaction with id: {transactionId} found.", exception.Message);
    }

    [TestMethod]
    [DataRow(99)]
    [DataRow(100)]
    public void GetById_TransactionNotFound_ThrowsExceptionWithMessage(int transactionId)
    {
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        transactionRepoMock.Setup(r => r.FindById(transactionId)).Returns((Transaction)null!);
        var transactionService = new TransactionService(transactionRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<TransactionNotFoundException>(() =>
            transactionService.GetById(transactionId)
        );
        Assert.AreEqual($"No transaction with id: {transactionId} found.", exception.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetById_ValidId_ReturnsTransaction(int transactionId)
    {
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var transaction = new Transaction(transactionId, "Test", 100.0, DateOnly.FromDateTime(DateTime.Today), 1, 1);
        transactionRepoMock.Setup(r => r.FindById(transactionId)).Returns(transaction);
        var transactionService = new TransactionService(transactionRepoMock.Object, loggerMock.Object);

        var result = transactionService.GetById(transactionId);

        Assert.IsNotNull(result, "Transaction should not be null");
        Assert.AreEqual(transactionId, result.Id, "Transaction id should match");
    }

    [TestMethod]
    public void GetById_DatabaseException_ThrowsExceptionWithMessage()
    {
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        transactionRepoMock.Setup(r => r.FindById(It.IsAny<int>())).Throws(new DatabaseException("DB error"));
        var transactionService = new TransactionService(transactionRepoMock.Object, loggerMock.Object);
        int testId = 123;

        var ex = Assert.ThrowsException<Exception>(() =>
            transactionService.GetById(testId)
        );
        Assert.AreEqual($"Database error retrieving transaction with id: {testId}", ex.Message);
    }

    [TestMethod]
    [DataRow(1, 1, 10)]
    [DataRow(2, 2, 5)]
    public void GetByUserIdPaged_DatabaseException_ThrowsExceptionWithMessage(int userId, int page, int pageSize)
    {
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        transactionRepoMock.Setup(r => r.FindByUserIdPaged(userId, page, pageSize))
            .Throws(new DatabaseException("DB error"));
        var transactionService = new TransactionService(transactionRepoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() =>
            transactionService.GetByUserIdPaged(userId, page, pageSize)
        );
        Assert.AreEqual("Database error while retrieving paged transactions.", ex.Message);
    }

    [TestMethod]
    [DataRow(1, 1, 10)]
    [DataRow(2, 2, 5)]
    public void GetByUserIdPaged_RepositoryReturnsNull_ThrowsExceptionWithMessage(int userId, int page, int pageSize)
    {
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        transactionRepoMock.Setup(r => r.FindByUserIdPaged(userId, page, pageSize)).Returns((List<Transaction>)null!);
        var transactionService = new TransactionService(transactionRepoMock.Object, loggerMock.Object);

        var exception = Assert.ThrowsException<Exception>(() =>
            transactionService.GetByUserIdPaged(userId, page, pageSize)
        );
        Assert.AreEqual(
            $"No transactions found. Object reference not set to an instance of an object.",
            exception.Message);
    }

    [TestMethod]
    [DataRow(1, 1, 10)]
    [DataRow(2, 2, 5)]
    public void GetByUserIdPaged_Valid_ReturnsTransactions(int userId, int page, int pageSize)
    {
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var transactionsList = new List<Transaction>
        {
            new Transaction(1, "Test", 100.0, DateOnly.FromDateTime(DateTime.Today), userId, 1)
        };
        transactionRepoMock.Setup(r => r.FindByUserIdPaged(userId, page, pageSize)).Returns(transactionsList);
        var transactionService = new TransactionService(transactionRepoMock.Object, loggerMock.Object);

        var result = transactionService.GetByUserIdPaged(userId, page, pageSize);

        Assert.IsNotNull(result, "Transactions should not be null");
        Assert.AreEqual(transactionsList.Count, result.Count, "Transaction count should match");
        Assert.AreEqual(transactionsList[0].Id, result[0].Id, "Transaction id should match");
    }

    [TestMethod]
    [DataRow("Test", 100.0, "2025-06-01", 1, 1)]
    public void Add_DatabaseException_ThrowsExceptionWithMessage(string description, double amount, string dateString,
        int userId, int categoryId)
    {
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        transactionRepoMock.Setup(r => r.Add(It.IsAny<Transaction>())).Throws(new DatabaseException("DB error"));
        var transactionService = new TransactionService(transactionRepoMock.Object, loggerMock.Object);
        DateOnly date = DateOnly.Parse(dateString);

        var ex = Assert.ThrowsException<Exception>(() =>
            transactionService.Add(description, amount, date, userId, categoryId)
        );
        Assert.AreEqual("Database error while adding a transaction.", ex.Message);
    }

    [TestMethod]
    [DataRow(1, 200.0, "Updated", "2025-06-01", 1, 1)]
    public void Edit_DatabaseException_ReturnsFalse(int id, double amount, string description, string dateString,
        int userId, int categoryId)
    {
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        transactionRepoMock.Setup(r => r.Edit(It.IsAny<Transaction>())).Throws(new DatabaseException("DB error"));
        var transactionService = new TransactionService(transactionRepoMock.Object, loggerMock.Object);
        DateOnly date = DateOnly.Parse(dateString);

        var result = transactionService.Edit(id, amount, description, date, userId, categoryId);

        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow(1)]
    public void Delete_DatabaseException_ReturnsFalse(int id)
    {
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        var transaction = new Transaction(id, "Test", 100.0, DateOnly.FromDateTime(DateTime.Today), 1, 1);
        transactionRepoMock.Setup(r => r.FindById(id)).Returns(transaction);
        transactionRepoMock.Setup(r => r.Delete(transaction)).Throws(new DatabaseException("DB error"));
        var transactionService = new TransactionService(transactionRepoMock.Object, loggerMock.Object);

        var result = transactionService.Delete(id);
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow(1)]
    public void GetBalance_DatabaseException_ThrowsExceptionWithMessage(int userId)
    {
        var loggerMock = new Mock<ILogger<TransactionService>>();
        var transactionRepoMock = new Mock<ITransactionRepository>();
        transactionRepoMock.Setup(r => r.FindByUserId(It.IsAny<int>())).Throws(new DatabaseException("DB error"));
        var transactionService = new TransactionService(transactionRepoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() =>
            transactionService.GetBalance(userId)
        );
        Assert.AreEqual($"Database error while calculating balance for user_id: {userId}", ex.Message);
    }
}