namespace RestAPI.RequestModels;

public class CategoryRequest(int userId, string name)
{
    public string Name { get; set; } = name;
    public int UserId { get; set; } = userId;
}