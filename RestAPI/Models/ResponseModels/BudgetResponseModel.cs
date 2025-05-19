namespace RestAPI.Models.ResponseModels;

public class BudgetResponseModel(string name, double amountSpent, double budgetAmount, string colorHexCode)
{
    public string Name { get; set; } = name;
    public double AmountSpent { get; set; } = amountSpent;
    public double BudgetAmount { get; set; } = budgetAmount;
    public string ColorHexCode { get; set; } = colorHexCode;
}