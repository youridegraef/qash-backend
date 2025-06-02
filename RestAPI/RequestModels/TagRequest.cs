namespace RestAPI.RequestModels;

public class TagRequest(string name, string colorHexCode, List<TransactionRequest> transactions)
{
    public string Name { get; set; } = name;
    public string ColorHexCode { get; set; } = colorHexCode;
}