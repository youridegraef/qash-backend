namespace RestAPI.Models;

public static class Requests
{
    public record Register(string Name, string Email, string Password, DateOnly DateOfBirth);

    public record Login(string Email, string Password);

    public record AddTransaction(string Description, double Amount, DateOnly Date, int UserId, CategoryModel Category, List<TagModel> Tags);
}