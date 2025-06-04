namespace RestAPI.RequestModels;

public class CategoryRequest(int id, string name, string colorHexCode) {
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string ColorHexCode { get; set; } = colorHexCode;
}