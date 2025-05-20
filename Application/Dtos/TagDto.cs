namespace Application.Dtos;

public class TagDto(int id, string name, string colorHexCode, int userId)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string ColorHexCode { get; set; } = colorHexCode;
    public int UserId { get; set; } = userId;
}