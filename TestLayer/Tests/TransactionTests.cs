using Application.Services;
using TestLayer.MockData;

namespace TestLayer.Tests;

[TestClass]
public class TransactionTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetById_IdZeroOrNegative_ThrowsException(int transactionId)
    {
        // Arrange
        var service = new TransactionService(new TransactionRepositoryMock(), new DummyLogger<TransactionService>());

        // Act & Assert
        var exception = Assert.ThrowsException<KeyNotFoundException>(() =>
            service.GetById(transactionId)
        );
        Assert.AreEqual($"No transaction with id: {transactionId} found.", exception.Message);
    }

    [TestMethod]
    [DataRow(99)]
    [DataRow(100)]
    public void GetById_TransactionNotFound_ThrowsException(int transactionId)
    {
        // Arrange
        var service = new TransactionService(new TransactionRepositoryMock(), new DummyLogger<TransactionService>());

        // Act & Assert
        var exception = Assert.ThrowsException<KeyNotFoundException>(() =>
            service.GetById(transactionId)
        );
        Assert.AreEqual($"No transaction with id: {transactionId} found.", exception.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetById_ValidId_ReturnsTransaction(int transactionId)
    {
        // Arrange
        var service = new TransactionService(new TransactionRepositoryMock(), new DummyLogger<TransactionService>());

        // Act
        var transaction = service.GetById(transactionId);

        // Assert
        Assert.IsNotNull(transaction, "Transaction should not be null");
        Assert.AreEqual(transactionId, transaction.Id, "Transaction id should match");
    }

    [TestMethod]
    public void GetAll_ReturnsAllTransactions()
    {
        // Arrange
        var service = new TransactionService(new TransactionRepositoryMock(), new DummyLogger<TransactionService>());

        // Act
        var transactions = service.GetAll();

        // Assert
        Assert.IsNotNull(transactions);
        Assert.IsTrue(transactions.Count > 0, "Should return at least one transaction");
    }

    [TestMethod]
    public void GetByUserId_ReturnsTransactionsForUser()
    {
        // Arrange
        var service = new TransactionService(new TransactionRepositoryMock(), new DummyLogger<TransactionService>());

        // Act
        var transactions = service.GetByUserId(1);

        // Assert
        Assert.IsNotNull(transactions);
        Assert.IsTrue(transactions.All(t => t.UserId == 1));
    }

    [TestMethod]
    public void GetByCategory_ReturnsTransactionsForCategory()
    {
        // Arrange
        var service = new TransactionService(new TransactionRepositoryMock(), new DummyLogger<TransactionService>());

        // Act
        var transactions = service.GetByCategory(1);

        // Assert
        Assert.IsNotNull(transactions);
        Assert.IsTrue(transactions.All(t => t.CategoryId == 1));
    }

    [TestMethod]
    public void Add_ValidTransaction_ReturnsTransactionWithId()
    {
        // Arrange
        var service = new TransactionService(new TransactionRepositoryMock(), new DummyLogger<TransactionService>());
        var date = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var transaction = service.Add("Test", 123.45, date, 1, 1);

        // Assert
        Assert.IsNotNull(transaction);
        Assert.IsTrue(transaction.Id > 0);
        Assert.AreEqual("Test", transaction.Description);
        Assert.AreEqual(123.45, transaction.Amount);
        Assert.AreEqual(date, transaction.Date);
        Assert.AreEqual(1, transaction.UserId);
        Assert.AreEqual(1, transaction.CategoryId);
    }

    [TestMethod]
    public void Edit_ValidTransaction_ReturnsTrue()
    {
        // Arrange
        var service = new TransactionService(new TransactionRepositoryMock(), new DummyLogger<TransactionService>());
        var date = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var result = service.Edit(1, 200.0, "Updated", date, 1, 1);

        // Assert
        Assert.IsTrue(result, "Edit should return true for valid transaction");
    }

    [TestMethod]
    public void Edit_TransactionNotFound_ReturnsFalse()
    {
        // Arrange
        var service = new TransactionService(new TransactionRepositoryMock(), new DummyLogger<TransactionService>());
        var date = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var result = service.Edit(999, 200.0, "Updated", date, 1, 1);

        // Assert
        Assert.IsFalse(result, "Edit should return false if transaction not found");
    }

    [TestMethod]
    public void Delete_ValidTransaction_ReturnsTrue()
    {
        // Arrange
        var service = new TransactionService(new TransactionRepositoryMock(), new DummyLogger<TransactionService>());

        // Act
        var result = service.Delete(1);

        // Assert
        Assert.IsTrue(result, "Delete should return true for valid transaction");
    }

    [TestMethod]
    public void Delete_TransactionNotFound_ReturnsFalse()
    {
        // Arrange
        var service = new TransactionService(new TransactionRepositoryMock(), new DummyLogger<TransactionService>());

        // Act
        var result = service.Delete(999);

        // Assert
        Assert.IsFalse(result, "Delete should return false if transaction not found");
    }

    [TestMethod]
    public void GetBalance_ReturnsCorrectBalance()
    {
        // Arrange
        var service = new TransactionService(new TransactionRepositoryMock(), new DummyLogger<TransactionService>());

        // Act
        var balance = service.GetBalance(1);

        // Assert
        // In de mock: 100.0 (Boodschappen) + (-50.0) (Huur) = 50.0
        Assert.AreEqual(50.0, balance, 0.01, "Balance should be correct for user 1");
    }

    [TestMethod]
    public void GetChartData_ReturnsChartData()
    {
        // Arrange
        var service = new TransactionService(new TransactionRepositoryMock(), new DummyLogger<TransactionService>());

        // Act
        var chartData = service.GetChartData(1);

        // Assert
        Assert.IsNotNull(chartData);
        Assert.IsTrue(chartData.Count > 0, "ChartData should not be empty");
    }
}