namespace RestAPI.ResponseModels;

public class BudgetResponse(
    int id,
    string name,
    double amountSpent,
    double budgetAmount,
    int userId) {
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public double AmountSpent { get; set; } = amountSpent;
    public double BudgetAmount { get; set; } = budgetAmount;
    public int UserId { get; set; } = userId;
}