namespace RestAPI.Models.ResponseModels;

public class TransactionResponseModel
{
    public string Description { get; set; }
    public double Amount { get; set; }
    public string Date { get; set; }
    public CategoryResponseModel Category { get; set; }
    public List<TagResponseModel> Tags { get; set; }
}