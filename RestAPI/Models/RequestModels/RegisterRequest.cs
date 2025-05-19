namespace RestAPI.Models.RequestModels;

public class RegisterRequest(string name, string email, string password, DateOnly dateOfBirth)
{
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
    public DateOnly DateOfBirth { get; set; } = dateOfBirth;
}