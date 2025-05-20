namespace RestAPI.ResponseModels;

public class TagResponse(int id, string name, string colorHexCode, List<TransactionResponse> transactions)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string ColorHexCode { get; set; } = colorHexCode;

    public List<TransactionResponse> Transactions { get; set; } = transactions;
}