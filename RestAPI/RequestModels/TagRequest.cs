namespace RestAPI.RequestModels;

public class TagRequest(string name, List<TransactionRequest> transactions) {
    public string Name { get; set; } = name;
}