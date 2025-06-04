namespace RestAPI.RequestModels;

public class TagRequest(string name)
{
    public string Name { get; set; } = name;
}