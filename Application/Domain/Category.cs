namespace Application.Domain;

public class Category {
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int UserId { get; private set; }

    public Category(string name, int userId) {
        Name = name;
        UserId = userId;
    }

    public Category(int id, string name, int userId) {
        Id = id;
        Name = name;
        UserId = userId;
    }
}