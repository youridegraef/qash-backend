using Application;
using Application.Exceptions;
using Application.Interfaces;
using Application.Domain;
using Application.Services;
using DataAccess.Repositories;

namespace ConsolePresentation;

class Program
{
    private static readonly IUserRepository userRepository = new UserRepository();
    private static readonly IUserService userService = new UserService(userRepository);

    static void Main(string[] args)
    {
        try
        {
            User authenticatdeUser1 = userService.Authenticate("youridegraef@icloud.com", "wachtwoord");
            Console.WriteLine(authenticatdeUser1.Name);
        }
        catch
        {
            throw new Exception("Invalid Credentials");
        }
        
        try
        {
            User authenticatdeUser2 = userService.Authenticate("youridegraef@icloud.com", "wachtwoord2");
            Console.WriteLine(authenticatdeUser2.Name);
        }
        catch
        {
            throw new Exception("Invalid Credentials");
        }
    }
}