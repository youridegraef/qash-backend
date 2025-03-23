using Application;
using Application.Exceptions;
using Application.Interfaces;
using Application.Domain;
using Application.Services;
using DataAccess.Repositories;

namespace ConsolePresentation;

class Program
{
    private static readonly IUserRepository _userRepository = new UserRepository();
    private static readonly IUserService _userService = new UserService(_userRepository);

    private static readonly ITransactionRepository _transactionRepository = new TransactionRepository();
    private static readonly ITransactionService _transactionService = new TransactionService(_transactionRepository);

    static void Main(string[] args)
    {
        Transaction transaction = _transactionService.Add(30.0, new DateOnly(2025, 3, 19), 1, 1);
        Console.WriteLine(transaction.Amount.ToString());
        Console.WriteLine(transaction.Date.ToString());
    }
}