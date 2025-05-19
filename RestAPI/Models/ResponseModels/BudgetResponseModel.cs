namespace RestAPI.Models.ResponseModels;

public class BudgetResponseModel
{
    public string Name { get; set; }
    public double AmountSpent { get; set; }
    public double BudgetAmount { get; set; }
    public string ColorHexCode { get; set; }
}