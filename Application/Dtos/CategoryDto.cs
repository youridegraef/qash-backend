namespace Application.Dtos;

public class CategoryDto(int id, string name, int userId, string colorHexCode)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public int UserId { get; set; } = userId;
    public string ColorHexCode { get; set; } = colorHexCode;
}