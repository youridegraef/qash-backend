using Application.Domain;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class TransactionService(
    ITransactionRepository transactionRepository,
    ILogger<TransactionService> logger)
    : ITransactionService
{
    public Transaction GetById(int id)
    {
        try
        {
            Transaction transaction = transactionRepository.FindById(id);

            if (transaction != null!)
            {
                return transaction;
            }

            throw new TransactionNotFoundException($"No transaction with id: {id} found.");
        }
        catch (TransactionNotFoundException ex)
        {
            logger.LogError(ex, "No transaction with id: {TransactionId} found.", id);
            throw;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving transaction with id: {TransactionId}", id);
            throw new Exception($"Database error retrieving transaction with id: {id}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving transaction with id: {TransactionId}", id);
            throw new Exception($"Error retrieving transaction with id: {id}", ex);
        }
    }

    public List<Transaction> GetAll()
    {
        try
        {
            List<Transaction> transactions = transactionRepository.FindAll();
            return transactions;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving all transactions.");
            throw new Exception("Database error while retrieving all transactions.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all transactions.");
            throw new Exception($"No transactions found. {ex.Message}", ex);
        }
    }

    public List<Transaction> GetByDateRange(DateOnly startDate, DateOnly endDate)
    {
        try
        {
            List<Transaction> allTransactions = transactionRepository.FindAll();
            var filteredTransactions = allTransactions
                .Where(t => t.Date >= startDate && t.Date <= endDate).ToList();

            return filteredTransactions;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving transactions in date range {StartDate} - {EndDate}.",
                startDate, endDate);
            throw new Exception($"Database error while retrieving transactions in date range {startDate} - {endDate}.",
                ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No transactions found in date range {StartDate} - {EndDate}.", startDate, endDate);
            throw new Exception($"No transactions found in date range {startDate} - {endDate}.", ex);
        }
    }

    public List<Transaction> GetByUserId(int userId)
    {
        try
        {
            List<Transaction> allTransactions = transactionRepository.FindAll();
            var filteredTransactions = allTransactions
                .Where(t => t.UserId == userId).ToList();

            return filteredTransactions;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving transactions with user_id: {UserId}", userId);
            throw new Exception($"Database error while retrieving transactions with user_id: {userId}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No transactions found with user_id: {UserId}", userId);
            throw new Exception($"No transactions found with user_id: {userId}", ex);
        }
    }

    public List<Transaction> GetByCategory(int id)
    {
        try
        {
            List<Transaction> allTransactions = transactionRepository.FindAll();
            var filteredTransactions = allTransactions
                .Where(t => t.CategoryId == id).ToList();

            return filteredTransactions;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error retrieving transactions with category id: {CategoryId}", id);
            throw new Exception($"Database error while retrieving transactions with category id: {id}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No transactions found with category id: {CategoryId}", id);
            throw new Exception($"No transactions found with category id: {id}", ex);
        }
    }

    public Transaction Add(string description, double amount, DateOnly date, int userId, int categoryId)
    {
        try
        {
            Transaction transaction = new Transaction(amount, description, date, userId, categoryId);
            transaction.Id = transactionRepository.Add(transaction);

            return transaction;
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Invalid transaction data: {Message}", ex.Message);
            throw new InvalidDataException("Invalid transaction data: " + ex.Message, ex);
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error while adding a transaction.");
            throw new Exception("Database error while adding a transaction.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while adding a transaction.");
            throw new Exception("An unexpected error occurred.", ex);
        }
    }

    public bool Edit(int id, double amount, string description, DateOnly date, int userId, int categoryId)
    {
        try
        {
            Transaction newTransaction = new Transaction(id, description, amount, date,
                userId, categoryId);

            return transactionRepository.Edit(newTransaction);
        }
        catch (TransactionNotFoundException ex)
        {
            logger.LogError(ex, "Transaction with ID {TransactionId} was not found for update.", id);
            return false;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error while updating transaction with ID {TransactionId}", id);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating transaction with ID {TransactionId}", id);
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            var transaction = transactionRepository.FindById(id);
            return transactionRepository.Delete(transaction);
        }
        catch (TransactionNotFoundException ex)
        {
            logger.LogError(ex, "Transaction with ID {TransactionId} was not found for deletion.", id);
            return false;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error while deleting transaction with ID {TransactionId}", id);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting transaction with ID {TransactionId}", id);
            return false;
        }
    }

    public double GetBalance(int userId)
    {
        try
        {
            double balance = 0.00;
            var transactions = transactionRepository.FindByUserId(userId);
            foreach (var transaction in transactions)
            {
                balance += transaction.Amount;
            }

            return balance;
        }
        catch (TransactionNotFoundException ex)
        {
            logger.LogError(ex, "No transactions found with user_id: {UserId}", userId);
            throw new TransactionNotFoundException($"No transactions found with user_id: {userId}", ex);
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error calculating balance for user_id: {UserId}", userId);
            throw new Exception($"Database error while calculating balance for user_id: {userId}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating balance for user_id: {UserId}", userId);
            throw new Exception($"Error calculating balance for user_id: {userId}", ex);
        }
    }

    public double GetExpenses(int userId)
    {
        try
        {
            double balance = 0.00;
            var transactions = transactionRepository.FindByUserId(userId);
            var filteredTransactions = transactions.Where(t => t.Amount < 0);
            foreach (var transaction in filteredTransactions)
            {
                balance += transaction.Amount;
            }

            return -balance;
        }
        catch (TransactionNotFoundException ex)
        {
            logger.LogError(ex, "No transactions found with user_id: {UserId}", userId);
            throw new TransactionNotFoundException($"No transactions found with user_id: {userId}", ex);
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error calculating balance for user_id: {UserId}", userId);
            throw new Exception($"Database error while calculating balance for user_id: {userId}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating balance for user_id: {UserId}", userId);
            throw new Exception($"Error calculating balance for user_id: {userId}", ex);
        }
    }

    public double GetIncome(int userId)
    {
        try
        {
            double balance = 0.00;
            var transactions = transactionRepository.FindByUserId(userId);
            var filteredTransactions = transactions.Where(t => t.Amount > 0);
            foreach (var transaction in filteredTransactions)
            {
                balance += transaction.Amount;
            }

            return balance;
        }
        catch (TransactionNotFoundException ex)
        {
            logger.LogError(ex, "No transactions found with user_id: {UserId}", userId);
            throw new TransactionNotFoundException($"No transactions found with user_id: {userId}", ex);
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error calculating balance for user_id: {UserId}", userId);
            throw new Exception($"Database error while calculating balance for user_id: {userId}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating balance for user_id: {UserId}", userId);
            throw new Exception($"Error calculating balance for user_id: {userId}", ex);
        }
    }

    public List<ChartDataDto> GetChartData(int userId)
    {
        try
        {
            List<ChartDataDto> chartData = new List<ChartDataDto>();
            double balance = 0;
            var transactions = GetByUserId(userId);

            var transactionsByDay = transactions
                .GroupBy(t => t.Date)
                .OrderBy(g => g.Key);

            foreach (var dayGroup in transactionsByDay)
            {
                double dailyChange = 0;
                foreach (var transaction in dayGroup)
                {
                    dailyChange += transaction.Amount;
                }

                balance += dailyChange;

                chartData.Add(new ChartDataDto(dayGroup.Key, balance));
            }

            return chartData;
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Database error generating chart data for user_id: {UserId}", userId);
            throw new Exception($"Database error while generating chart data for user_id: {userId}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating chart data for user_id: {UserId}", userId);
            throw new Exception($"Error generating chart data for user_id: {userId}", ex);
        }
    }
}