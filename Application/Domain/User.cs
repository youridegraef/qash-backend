using Application.Domain;

public class User {
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }

    public User() { }

    public User(string name, string email, string passwordHash) {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
    }

    public User(int id, string name, string email, string passwordHash) {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
    }
}