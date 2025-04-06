using System.Net.Mail;
using System.Security.Authentication;
using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;

    public UserService(IUserRepository userRepository, ITransactionRepository transactionRepository)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
    }


    public User Register(string name, string email, string password, DateOnly dateOfBirth)
    {
        try
        {
            MailAddress m = new MailAddress(email);
            string hashedPassword = PasswordHasher.HashPassword(password);
            User newUser = new User(name, email, hashedPassword, dateOfBirth);
            _userRepository.Add(newUser);
            return newUser;
        }
        catch (FormatException ex)
        {
            //log the error.
            Console.WriteLine($"Invalid email format: {ex.Message}");
            throw new InvalidEmailFormatException("Invalid email format");
        }
        catch (Exception ex)
        {
            //log the error.
            Console.WriteLine($"Error registering user: {ex.Message}, {ex.StackTrace}");
            throw new Exception("Error registering user");
        }
    }

    public User Authenticate(string email, string password)
    {
        User? user = _userRepository.FindByEmail(email);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (PasswordHasher.VerifyPassword(password, user.PasswordHash))
        {
            return user;
        }

        throw new AuthenticationException("Invalid email or password");
    }

    public User GetById(int userId)
    {
        User? user = _userRepository.FindById(userId);

        if (user != null)
        {
            return user;
        }

        throw new UserNotFoundException("User not found");
    }

    public User GetByEmail(string email)
    {
        User? user = _userRepository.FindByEmail(email);

        if (user != null)
        {
            return user;
        }

        throw new UserNotFoundException("User not found");
    }

    public bool Update(User user)
    {
        try
        {
            _userRepository.Edit(user);
            return true;
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
            User user = _userRepository.FindById(id);
            _userRepository.Delete(user);
            return true;
        }
        catch (UserNotFoundException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public double CalculateBalance(int userId)
    {
        double balance = 0;

        List<Transaction> transactions = _transactionRepository.FindAll();
        var filteredTransactions = transactions.Where(t => t.UserId == userId).ToList();

        foreach (var transaction in filteredTransactions)
        {
            balance += transaction.Amount;
        }

        return balance;
    }

    public double CalculateIncome(int userId)
    {
        double income = 0;

        List<Transaction> transactions = _transactionRepository.FindAll();
        var filteredTransactions = transactions.Where(t => t.UserId == userId && t.Amount > 00.00).ToList();

        foreach (var transaction in filteredTransactions)
        {
            income += transaction.Amount;
        }

        return income;
    }

    public double CalculateExpenses(int userId)
    {
        double expenses = 0;

        List<Transaction> transactions = _transactionRepository.FindAll();
        var filteredTransactions = transactions.Where(t => t.UserId == userId && t.Amount < 00.00).ToList();

        foreach (var transaction in filteredTransactions)
        {
            expenses += transaction.Amount;
        }


        if (expenses == 0)
        {
            return expenses;
        }

        return expenses * -1;
    }
}