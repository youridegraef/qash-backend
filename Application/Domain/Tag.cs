namespace Application.Domain;

public class Tag
{
    public int Id { get; private set; }
    public string Name { get; set; }
    public string ColorHexCode { get; set; }
    public int UserId { get; set; }

    public Tag(int id, string name, string colorHexCode, int userId)
    {
        Id = id;
        Name = name;
        ColorHexCode = colorHexCode;
        UserId = userId;
    }

    public Tag(string name)
    {
        Name = name;
        
        Random random = new Random();
        int red = random.Next(256);
        int green = random.Next(256);
        int blue = random.Next(256);

        ColorHexCode = $"#{red:X2}{green:X2}{blue:X2}";
    }
}