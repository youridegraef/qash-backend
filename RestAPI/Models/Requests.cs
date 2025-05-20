namespace RestAPI.Models;

public static class Requests
{
    public record Register(string Name, string Email, string Password, DateOnly DateOfBirth);

    public record Login(string Email, string Password);
}