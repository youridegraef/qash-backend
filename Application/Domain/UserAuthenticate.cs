using Application.Domain;

public class UserAuthenticate
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; private set; }
    public DateOnly DateOfBirth { get; set; }

    public UserAuthenticate()
    {
    }

    public UserAuthenticate(string name, string email, string passwordHash, DateOnly dateOfBirth)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        DateOfBirth = dateOfBirth;
    }

    public UserAuthenticate(int id, string name, string email, string passwordHash, DateOnly dateOfBirth)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        DateOfBirth = dateOfBirth;
    }
}