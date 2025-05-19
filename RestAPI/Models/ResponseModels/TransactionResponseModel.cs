namespace RestAPI.Models.ResponseModels;

public class TransactionResponseModel(
    string description,
    double amount,
    string date,
    CategoryResponseModel category,
    List<TagResponseModel> tags)
{
    public string Description { get; set; } = description;
    public double Amount { get; set; } = amount;
    public string Date { get; set; } = date;
    public CategoryResponseModel Category { get; set; } = category;
    public List<TagResponseModel> Tags { get; set; } = tags;
}