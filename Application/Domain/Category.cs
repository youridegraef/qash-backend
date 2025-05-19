namespace Application.Domain;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UserId { get; set; }
    public string ColorHexCode { get; set; }

    public Category(string name, int userId, string colorHexCode)
    {
        Name = name;
        UserId = userId;
        ColorHexCode = colorHexCode;
    }

    public Category(int id, string name, int userId, string colorHexCode)
    {
        Id = id;
        Name = name;
        UserId = userId;
        ColorHexCode = colorHexCode;
    }
}