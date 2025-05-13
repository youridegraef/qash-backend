namespace RestAPI.Models;

public class UserModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateOnly DateOfBirth { get; set; }

    public UserModel(int id, string name, string email, DateOnly dateOfBirth)
    {
        Id = id;
        Name = name;
        Email = email;
        DateOfBirth = dateOfBirth;
    }
}