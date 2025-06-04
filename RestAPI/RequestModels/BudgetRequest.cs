using Application.Domain;

namespace RestAPI.RequestModels;

public class BudgetRequest(
    int id,
    string name,
    decimal amountSpent,
    decimal budgetAmount,
    string colorHexCode,
    int userId,
    User user) {
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public decimal AmountSpent { get; set; } = amountSpent;
    public decimal BudgetAmount { get; set; } = budgetAmount;
    public string ColorHexCode { get; set; } = colorHexCode;

    public int UserId { get; set; } = userId;
    public User User { get; set; } = user;
}