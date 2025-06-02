using Application.Domain;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class TransactionService(
    ITransactionRepository transactionRepository,
    ITagService tagService,
    ICategoryService categoryService,
    ILogger<TransactionService> logger)
    : ITransactionService {
    public TransactionDto GetById(int id) {
        try {
            var transaction = transactionRepository.FindById(id);

            if (transaction == null) {
                throw new TransactionNotFoundException($"No transaction with id: {id} found.");
            }

            var category = categoryService.GetById(transaction.CategoryId);
            var tags = tagService.GetByTransactionId(id);

            var dto = new TransactionDto(transaction.Id, transaction.Description, transaction.Amount,
                transaction.Date, transaction.UserId, transaction.CategoryId, category, tags);

            return dto;
        }
        catch (TransactionNotFoundException ex) {
            logger.LogError(ex, "No transaction with id: {TransactionId} found.", id);
            throw;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error retrieving transaction with id: {TransactionId}", id);
            throw new Exception($"Database error retrieving transaction with id: {id}", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error retrieving transaction with id: {TransactionId}", id);
            throw new Exception($"Error retrieving transaction with id: {id}", ex);
        }
    }

    public List<TransactionDto> GetByUserIdPaged(int userId, int page, int pageSize) {
        try {
            var transactions = transactionRepository.FindByUserIdPaged(userId, page, pageSize);
            List<TransactionDto> dtos = new List<TransactionDto>();

            if (transactions.Count == 0 || transactions == null) {
                throw new TransactionNotFoundException($"Transaction with UserID {userId} was not found.");
            }

            foreach (var transaction in transactions) {
                var category = categoryService.GetById(transaction.CategoryId);
                var tags = tagService.GetByTransactionId(transaction.Id);

                dtos.Add(
                    new TransactionDto(
                        transaction.Id, transaction.Description, transaction.Amount,
                        transaction.Date, transaction.UserId, transaction.CategoryId, category, tags
                    )
                );
            }

            return dtos;
        }
        catch (TransactionNotFoundException ex) {
            logger.LogError(ex, $"Transaction with UserID {userId} was not found.");
            throw new TransactionNotFoundException($"No transaction with user id: {userId} found.");
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error retrieving paged transactions.");
            throw new Exception("Database error while retrieving paged transactions.", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error retrieving paged transactions.");
            throw new Exception($"No transactions found. {ex.Message}", ex);
        }
    }

    public List<TransactionDto> GetByUserId(int userId) {
        try {
            List<Transaction> transactions = transactionRepository.FindByUserId(userId);

            List<TransactionDto> dtos = new List<TransactionDto>();

            if (transactions.Count == 0 || transactions == null) {
                throw new TransactionNotFoundException($"Transaction with UserID {userId} was not found.");
            }

            foreach (var transaction in transactions) {
                var category = categoryService.GetById(transaction.CategoryId);
                var tags = tagService.GetByTransactionId(transaction.Id);

                dtos.Add(
                    new TransactionDto(
                        transaction.Id, transaction.Description, transaction.Amount,
                        transaction.Date, transaction.UserId, transaction.CategoryId, category, tags
                    )
                );
            }

            return dtos;
        }
        catch (TransactionNotFoundException ex) {
            logger.LogError(ex, $"Transaction with UserID {userId} was not found.");
            throw new TransactionNotFoundException($"No transaction with user id: {userId} found.");
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error retrieving transactions with user_id: {UserId}", userId);
            throw new Exception($"Database error while retrieving transactions with user_id: {userId}", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "No transactions found with user_id: {UserId}", userId);
            throw new Exception($"No transactions found with user_id: {userId}", ex);
        }
    }

    public TransactionDto Add(string description, double amount, DateOnly date, int userId, int categoryId,
        List<Tag> tags) {
        try {
            if (string.IsNullOrWhiteSpace(description)) {
                throw new ArgumentException("Description cannot be empty.");
            }

            if (userId <= 0) {
                throw new ArgumentException("User ID must be greater than zero.");
            }

            if (categoryId <= 0) {
                throw new ArgumentException("Category ID must be greater than zero.");
            }

            var newTransaction = new Transaction(amount, description, date, userId, categoryId);
            var addedTransaction = transactionRepository.Add(newTransaction);

            if (addedTransaction == null) {
                throw new InvalidOperationException("Transaction was not added.");
            }

            AddTagsToTransaction(addedTransaction.Id, tags);

            var category = categoryService.GetById(addedTransaction.CategoryId);
            if (category == null) {
                throw new InvalidOperationException("Category not found.");
            }

            var dto = new TransactionDto(addedTransaction.Id, addedTransaction.Description, addedTransaction.Amount,
                addedTransaction.Date, addedTransaction.UserId, addedTransaction.CategoryId, category, tags);

            return dto;
        }
        catch (ArgumentException ex) {
            logger.LogError(ex, "Invalid transaction data: {Message}", ex.Message);
            throw new InvalidDataException("Invalid transaction data: " + ex.Message, ex);
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error while adding a transaction.");
            throw new Exception("Database error while adding a transaction.", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "An unexpected error occurred while adding a transaction.");
            throw new Exception("An unexpected error occurred.", ex);
        }
    }

    public bool Edit(int id, double amount, string description, DateOnly date, int userId, int categoryId,
        List<Tag> tags) {
        try {
            Transaction newTransaction = new Transaction(id, description, amount, date,
                userId, categoryId);
            AddTagsToTransaction(id, tags);

            return transactionRepository.Edit(newTransaction);
        }
        catch (TransactionNotFoundException ex) {
            logger.LogError(ex, "Transaction with ID {TransactionId} was not found for update.", id);
            return false;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error while updating transaction with ID {TransactionId}", id);
            return false;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error updating transaction with ID {TransactionId}", id);
            return false;
        }
    }

    public bool Delete(int id) {
        try {
            var transaction = transactionRepository.FindById(id);
            return transactionRepository.Delete(new Transaction(
                transaction.Id,
                transaction.Description,
                transaction.Amount,
                transaction.Date,
                transaction.UserId,
                transaction.CategoryId
            ));
        }
        catch (TransactionNotFoundException ex) {
            logger.LogError(ex, "Transaction with ID {TransactionId} was not found for deletion.", id);
            return false;
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error while deleting transaction with ID {TransactionId}", id);
            return false;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error deleting transaction with ID {TransactionId}", id);
            return false;
        }
    }

    public double GetBalance(int userId) {
        try {
            double balance = 0.00;
            var transactions = transactionRepository.FindByUserId(userId);
            foreach (var transaction in transactions) {
                balance += transaction.Amount;
            }

            return balance;
        }
        catch (TransactionNotFoundException ex) {
            logger.LogError(ex, "No transactions found with user_id: {UserId}", userId);
            throw new TransactionNotFoundException($"No transactions found with user_id: {userId}", ex);
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error calculating balance for user_id: {UserId}", userId);
            throw new Exception($"Database error while calculating balance for user_id: {userId}", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error calculating balance for user_id: {UserId}", userId);
            throw new Exception($"Error calculating balance for user_id: {userId}", ex);
        }
    }

    public double GetExpenses(int userId) {
        try {
            double balance = 0.00;
            var transactions = transactionRepository.FindByUserId(userId);
            var filteredTransactions = transactions.Where(t => t.Amount < 0);
            foreach (var transaction in filteredTransactions) {
                balance += transaction.Amount;
            }

            return -balance;
        }
        catch (TransactionNotFoundException ex) {
            logger.LogError(ex, "No transactions found with user_id: {UserId}", userId);
            throw new TransactionNotFoundException($"No transactions found with user_id: {userId}", ex);
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error calculating balance for user_id: {UserId}", userId);
            throw new Exception($"Database error while calculating balance for user_id: {userId}", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error calculating balance for user_id: {UserId}", userId);
            throw new Exception($"Error calculating balance for user_id: {userId}", ex);
        }
    }

    public double GetIncome(int userId) {
        try {
            double balance = 0.00;
            var transactions = transactionRepository.FindByUserId(userId);
            var filteredTransactions = transactions.Where(t => t.Amount > 0);
            foreach (var transaction in filteredTransactions) {
                balance += transaction.Amount;
            }

            return balance;
        }
        catch (TransactionNotFoundException ex) {
            logger.LogError(ex, "No transactions found with user_id: {UserId}", userId);
            throw new TransactionNotFoundException($"No transactions found with user_id: {userId}", ex);
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error calculating balance for user_id: {UserId}", userId);
            throw new Exception($"Database error while calculating balance for user_id: {userId}", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error calculating balance for user_id: {UserId}", userId);
            throw new Exception($"Error calculating balance for user_id: {userId}", ex);
        }
    }

    private void AddTagsToTransaction(int transactionId, List<Tag> tags) {
        try {
            transactionRepository.AddTagsToTransaction(transactionId, tags);
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Database error while adding tags to transaction.");
            throw new Exception("Database error while adding a transaction.", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "An unexpected error occurred while adding tags to transaction.");
            throw new Exception("An unexpected error occurred.", ex);
        }
    }
}