using Application;
using Application.Interfaces;
using Application.Services;
using Application.Domain;
using DataAccess;
using DataAccess.Repositories;

namespace Test;

class Program
{
    static void Main(string[] args)
    {
        DatabaseConnection dbConnection = new DatabaseConnection("Data Source=database.db");
        IUserRepository userRepository = new UserRepository(dbConnection);
        UserService userService = new UserService(userRepository);

        userService.RegisterUser("Name", "Email", "bingo", new DateOnly(2006, 2, 22));
    }
}