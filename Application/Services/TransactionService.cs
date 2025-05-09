using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }


    public Transaction GetById(int id)
    {
        Transaction transaction = _transactionRepository.FindById(id);

        if (transaction != null)
        {
            return transaction;
        }

        throw new KeyNotFoundException($"No transaction with id: {id} found.");
    }

    public List<Transaction> GetAll()
    {
        try
        {
            return _transactionRepository.FindAll();
        }
        catch (Exception)
        {
            throw new Exception($"No transactions found.");
        }
    }

    public List<Transaction> GetByDateRange(DateOnly startDate, DateOnly endDate)
    {
        try
        {
            List<Transaction> allTransactions = _transactionRepository.FindAll();
            var filteredTransactions = allTransactions
                .Where(t => t.Date >= startDate && t.Date <= endDate).ToList();
            return filteredTransactions;
        }
        catch (Exception)
        {
            throw new Exception($"No transactions found.");
        }
    }

    public List<Transaction> GetByUserId(int userId)
    {
        try
        {
            List<Transaction> allTransactions = _transactionRepository.FindAll();
            var filteredTransactions = allTransactions
                .Where(t => t.UserId == userId).ToList();

            return filteredTransactions;
        }
        catch (Exception)
        {
            throw new Exception($"No transactions found with user_id: {userId}");
        }
    }

    public List<Transaction> GetByPeriod(DateOnly start, DateOnly end)
    {
        try
        {
            List<Transaction> allTransactions = _transactionRepository.FindAll();
            var filteredTransactions = allTransactions
                .Where(x => x.Date >= start && x.Date <= end).ToList();

            return filteredTransactions;
        }
        catch (Exception)
        {
            throw new Exception($"No transactions found with specified period: {start.ToString()} - {end.ToString()}");
        }
    }

    public List<Transaction> GetByCategory(int id)
    {
        try
        {
            List<Transaction> allTransactions = _transactionRepository.FindAll();
            var filteredTransactions = allTransactions
                .Where(t => t.CategoryId == id).ToList();

            return filteredTransactions;
        }
        catch (Exception)
        {
            throw new Exception($"No transactions found with category id: {id}");
        }
    }

    public List<Transaction> GetByTag(int tagId)
    {
        try
        {
            List<Transaction> allTransactions = _transactionRepository.FindAll();

            var filteredTransactions = allTransactions
                .Where(t => t.Tags != null && t.Tags.Any(tag => tag.Id == tagId))
                .ToList();

            return filteredTransactions;
        }
        catch
        {
            throw new Exception($"No transactions under tag with id: {tagId} found.");
        }
    }

    public Transaction Add(string description, double amount, DateOnly date, int userId, int categoryId)
    {
        try
        {
            Transaction transaction = new Transaction(amount, description, date, userId, categoryId);
            transaction.Id = _transactionRepository.Add(transaction);
            return transaction;
        }
        catch (ArgumentException ex)
        {
            throw new InvalidDataException("Invalid transaction data: " + ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred.", ex);
        }
    }

    public bool Edit(int id, double amount, string description, DateOnly date, int userId, int categoryId)
    {
        try
        {
            Transaction newTransaction = new Transaction(amount, description, date,
                userId, categoryId);

            return _transactionRepository.Edit(newTransaction);
        }
        catch (TransactionNotFoundException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            var transaction = _transactionRepository.FindById(id);
            return _transactionRepository.Delete(transaction);
        }
        catch (TransactionNotFoundException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool AssignTag(int transactionId, int tagId)
    {
        throw new NotImplementedException();
    }

    public bool UnAssignTag(int transactionId, int tagId)
    {
        throw new NotImplementedException();
    }
}