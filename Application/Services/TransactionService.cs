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
    
    //TODO: TEST ALL METHODS

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

    public List<Transaction> GetAllByUserId(int userId)
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
        throw new NotImplementedException();
    }

    public Transaction Add(double amount, DateOnly date, int userId, int categoryId)
    {
        try
        {
            Transaction transaction = new Transaction(amount, date, userId, categoryId);
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

    public bool Edit(Transaction transaction)
    {
        try
        {
            return _transactionRepository.Edit(transaction);
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
        try
        {
            return _transactionRepository.AssignTagToTransaction(transactionId, tagId);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool UnAssignTag(int transactionId, int tagId)
    {
        try
        {
            return _transactionRepository.DeleteTagFromTransaction(transactionId, tagId);
        }
        catch (Exception)
        {
            return false;
        }
    }
}