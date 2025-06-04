namespace RestAPI.ResponseModels;

public class CategoryResponse(int id, string name)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
}