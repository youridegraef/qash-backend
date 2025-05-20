namespace RestAPI.RequestModels;

public class TagRequest(int id, string name, string colorHexCode, List<TransactionRequest> transactions)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string ColorHexCode { get; set; } = colorHexCode;

    public List<TransactionRequest> Transactions { get; set; } = transactions;
}