namespace Application.Domain;

public class Tag {
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int UserId { get; private set; }

    public Tag(int id, string name, int userId) {
        Id = id;
        Name = name;
        UserId = userId;
    }

    public Tag(string name, int userId) {
        Name = name;
        UserId = userId;
    }

    public Tag(string name) {
        Name = name;
    }

    public Tag(int id, string name) {
        Id = id;
        Name = name;
    }
}