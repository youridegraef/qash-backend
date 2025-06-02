namespace Application.Domain;

public class Category {
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int UserId { get; private set; }
    public string ColorHexCode { get; private set; }

    public Category(string name, int userId, string colorHexCode) {
        Name = name;
        UserId = userId;
        ColorHexCode = colorHexCode;
    }

    public Category(int id, string name, int userId, string colorHexCode) {
        Id = id;
        Name = name;
        UserId = userId;
        ColorHexCode = colorHexCode;
    }
}