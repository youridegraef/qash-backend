namespace RestAPI.Models;

public class BudgetModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal AmountSpent { get; set; }
    public decimal BudgetAmount { get; set; }
    public string ColorHexCode { get; set; }
        
    public int UserId { get; set; }
    public User User { get; set; }
}