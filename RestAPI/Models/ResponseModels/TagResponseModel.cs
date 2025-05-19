namespace RestAPI.Models.ResponseModels;

public class TagResponseModel(string name, string colorHexCode)
{
    public string Name { get; set; } = name;
    public string ColorHexCode { get; set; } = colorHexCode;
}