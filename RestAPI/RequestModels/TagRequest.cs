namespace RestAPI.RequestModels;

public class TagRequest(string name, int userId)
{
    public string Name { get; set; } = name;
    public int UserId { get; set; } = userId;
}