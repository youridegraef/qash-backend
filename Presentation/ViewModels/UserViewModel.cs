using Application.Domain;

namespace Presentation.ViewModels;

public class UserViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; private set; }
    public DateOnly DateOfBirth { get; set; }

    public UserViewModel()
    {
    }

    public UserViewModel(string name, string email, string passwordHash, DateOnly dateOfBirth)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        DateOfBirth = dateOfBirth;
    }
}