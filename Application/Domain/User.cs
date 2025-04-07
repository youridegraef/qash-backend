using Application.Domain;

public class User
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; private set; }
    public DateOnly DateOfBirth { get; set; }

    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    public List<Tag> Tags { get; set; } = new List<Tag>();
    public List<SavingGoal> SavingGoals { get; set; } = new List<SavingGoal>();
    public List<Category> Categories { get; set; } = new List<Category>();

    public User()
    {
    }

    public User(string name, string email, string passwordHash, DateOnly dateOfBirth)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        DateOfBirth = dateOfBirth;
    }

    public User(long id, string name, string email, string passwordHash, DateOnly dateOfBirth)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        DateOfBirth = dateOfBirth;
    }
}